using System;
using System.Collections.Generic;
using System.Linq;
using CommonElements;
using EditorExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class GraphRewriteMainPanel : VisualElement
{
    private SerializedObject generationDataSerializedObject;

    private GraphListView graphsListView;
    private RuleListView rulesListView;

    private VisualElement rightPanel;
    private GraphRewriteView currentGraphView;
    private RuleGraphPairView currentRuleGraphPairView;

    private IntegerField seedNumberField;
    private GraphRewriteSimulateView graphRewriteSimulateView;
    private DropdownButton startGraphDropdownButton;
    private ToolbarToggle simulateToggle;

    private GenerationData GenerationData =>
        generationDataSerializedObject.targetObject as GenerationData;

    public GraphRewriteMainPanel()
    {
        this.AddUXMLByName();

        var toolbar = new DevGraphToolbar();
        toolbar.SaveButton.clicked += SaveGraph;

        simulateToggle = new ToolbarToggle
        {
            text = "Simulate"
        };
        simulateToggle.RegisterValueChangedCallback(OnSimulateToggleValueChanged);

        seedNumberField = new IntegerField("Seed");

        var startGraphContainer = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };
        var label = new Label("Start Graph");
        startGraphDropdownButton = new DropdownButton
        {
            DropdownGetter = () =>
                             {
                                 return GenerationData ?
                                            GenerationData.Graphs.Select(_ => _.id) :
                                            Array.Empty<string>();
                             }
        };
        startGraphContainer.Add(label);
        startGraphContainer.Add(startGraphDropdownButton);

        toolbar.PostSaveContainer.Add(simulateToggle);
        toolbar.PostSaveContainer.Add(seedNumberField);
        toolbar.PostSaveContainer.Add(startGraphContainer);
        Insert(0, toolbar);

        rightPanel = this.Q<VisualElement>("right-container");

        graphsListView = this.Q<GraphListView>();
        graphsListView.OnSelectionChanged = OnGraphSelected;

        rulesListView = this.Q<RuleListView>();
        rulesListView.OnSelectionChanged = OnRuleSelected;

        graphRewriteSimulateView = new GraphRewriteSimulateView
        {
            style =
            {
                flexGrow = 1
            }
        };
    }

    private IEnumerable<string> OnStartGraphSelected()
    {
        throw new NotImplementedException();
    }

    private void OnSimulateToggleValueChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue == false)
            return;

        graphsListView.ClearSelection();
        rulesListView.ClearSelection();

        rightPanel.Clear();

        rightPanel.Add(graphRewriteSimulateView);
    }

    private void OnGraphSelected(IEnumerable<object> selectedItems)
    {
        if (selectedItems.FirstOrDefault() is GraphData graphData)
        {
            rulesListView.ClearSelection();
            simulateToggle.value = false;

            rightPanel.Clear();

            currentGraphView = new GraphRewriteView
            {
                style =
                {
                    flexGrow = 1
                }
            };

            SerializedProperty graphSO =
                generationDataSerializedObject.FindProperty(
                                                   GUIUtils.GetBackingFieldName(
                                                       nameof(GenerationData.Graphs))).
                                               GetArrayElementAtIndex(
                                                   graphsListView.selectedIndex);

            currentGraphView.LoadGraph(graphSO);
            rightPanel.Add(currentGraphView);
        }
    }

    private void OnRuleSelected(IEnumerable<object> selectedItems)
    {
        if (selectedItems.FirstOrDefault() is RuleData ruleData)
        {
            graphsListView.ClearSelection();
            simulateToggle.value = false;

            rightPanel.Clear();

            currentRuleGraphPairView = new RuleGraphPairView
            {
                style =
                {
                    flexGrow = 1
                }
            };
            currentRuleGraphPairView.Setup(generationDataSerializedObject,
                                           rulesListView.selectedIndex);
            rightPanel.Add(currentRuleGraphPairView);
        }
    }

    private void SaveGraph()
    {
        if (generationDataSerializedObject == null)
            return;

        AssetDatabase.SaveAssetIfDirty(generationDataSerializedObject.targetObject);
        AssetDatabase.Refresh();
    }

    public void LoadGraph(SerializedObject genDataSerializedObject)
    {
        generationDataSerializedObject = genDataSerializedObject;

        if (generationDataSerializedObject == null)
            return;

        graphsListView.Setup(generationDataSerializedObject);

        rulesListView.Setup(generationDataSerializedObject);
        graphRewriteSimulateView.Setup(generationDataSerializedObject);

        SerializedProperty seedProperty =
            generationDataSerializedObject.FindProperty(
                GUIUtils.GetBackingFieldName(nameof(GenerationData.Seed)));
        seedNumberField.BindProperty(seedProperty);
        seedNumberField.Bind(generationDataSerializedObject);

        SerializedProperty startGraphProperty =
            generationDataSerializedObject.FindProperty(
                GUIUtils.GetBackingFieldName(nameof(GenerationData.StartGraphID)));
        startGraphDropdownButton.BindProperty(startGraphProperty);
        startGraphDropdownButton.Bind(generationDataSerializedObject);
    }

    public void Update()
    {
        currentGraphView?.UpdatePositions();
        currentRuleGraphPairView?.UpdatePositions();
        graphRewriteSimulateView?.UpdatePositions();
    }
}
