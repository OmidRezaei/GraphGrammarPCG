using System;
using System.Collections.Generic;
using EditorExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class GraphListView : ListView
{
    public new class UxmlFactory : UxmlFactory<GraphListView, UxmlTraits>
    {
    }

    private SerializedObject genDataSO;

    private GenerationData GenData => genDataSO.targetObject as GenerationData;
    public Action<IEnumerable<object>> OnSelectionChanged { get; set; }

    public GraphListView()
    {
    }

    public GraphListView(SerializedObject generationData)
    {
        Setup(generationData);
    }

    public void Setup(SerializedObject generationData)
    {
        genDataSO = generationData;
        itemsSource = GenData.Graphs;

        makeItem = MakeItem;
        bindItem = BindItem;
        selectionChanged += _ => OnSelectionChanged?.Invoke(_);
    }

    private void BindItem(VisualElement element, int index)
    {
        SerializedObject so = genDataSO;

        SerializedProperty propertyAtIndex =
            so.FindProperty(
                   GUIUtils.GetBackingFieldName(
                       nameof(GenerationData.Graphs))).
               GetArrayElementAtIndex(index).
               FindPropertyRelative(nameof(GraphData.id));

        TextField textField = element.Q<TextField>();
        textField.BindProperty(propertyAtIndex);
        textField.Bind(so);
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

        return element;
    }
}
