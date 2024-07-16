using System;
using System.Collections.Generic;
using System.Linq;
using EditorExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class RuleListView : ListView
{
    public new class UxmlFactory : UxmlFactory<RuleListView, UxmlTraits>
    {
    }

    private SerializedObject genDataSO;

    private GenerationData GenData => genDataSO.targetObject as GenerationData;
    public Action<IEnumerable<object>> OnSelectionChanged { get; set; }

    public RuleListView()
    {
    }

    public RuleListView(SerializedObject generationData)
    {
        Setup(generationData);
    }

    public void Setup(SerializedObject generationData)
    {
        genDataSO = generationData;
        itemsSource = GenData!.Rules;

        makeItem = MakeItem;
        bindItem = BindItem;
        selectionChanged += _ => OnSelectionChanged?.Invoke(_);
    }

    private void BindItem(VisualElement element, int index)
    {
        SerializedObject so = genDataSO;

        SerializedProperty ruleProperty = so.FindProperty(
                                                 GUIUtils.GetBackingFieldName(
                                                     nameof(GenData.Rules))).
                                             GetArrayElementAtIndex(index);
        SerializedProperty idProperty =
            ruleProperty.FindPropertyRelative(nameof(RuleData.id));
        SerializedProperty sourceProperty =
            ruleProperty.FindPropertyRelative(nameof(RuleData.sourceGraph));
        SerializedProperty targetProperty =
            ruleProperty.FindPropertyRelative(nameof(RuleData.targetGraph));

        TextField textField = element.Q<TextField>();
        textField.BindProperty(idProperty);
        textField.Bind(so);

        DropdownButton sourceDropdownButton = element.Q<DropdownButton>("source-id-dropdown");
        DropdownButton targetDropdownButton = element.Q<DropdownButton>("target-id-dropdown");
        sourceDropdownButton.SetActiveItem(sourceProperty.stringValue);
        targetDropdownButton.SetActiveItem(targetProperty.stringValue);
        sourceDropdownButton.OnActiveItemChanged +=
            s =>
            {
                sourceProperty.stringValue = s;
                sourceProperty.serializedObject.ApplyModifiedProperties();
            };
        targetDropdownButton.OnActiveItemChanged +=
            s =>
            {
                targetProperty.stringValue = s;
                targetProperty.serializedObject.ApplyModifiedProperties();
            };
    }

    private VisualElement MakeItem()
    {
        var element = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };

        var textField = new TextField();
        element.Add(textField);

        var sourceGraph = new DropdownButton
        {
            name = "source-id-dropdown",
            DropdownGetter = () => GenData.Graphs.Select(_ => _.id)
        };
        var targetGraph = new DropdownButton
        {
            name = "target-id-dropdown",
            DropdownGetter = () => GenData.Graphs.Select(_ => _.id)
        };

        element.Add(sourceGraph);
        element.Add(targetGraph);

        return element;
    }
}
