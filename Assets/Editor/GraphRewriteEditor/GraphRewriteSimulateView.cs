using System.Linq;
using CommonEditorElements.Graph;
using EditorExtensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphRewriteSimulateView : CommonGraphView
{
    private ForceDirectedGraphManipulator forceDirectedGraphManipulator;

    private SerializedObject genDataSO;

    private SerializedProperty nodesProperty;
    private SerializedProperty edgesProperty;
    private SerializedObject simulationSO;

    private EditorSimulationData simulationData;
    private SimulationOptions simulationOptions;

    public GenerationData GenData => genDataSO.targetObject as GenerationData;

    public GraphRewriteSimulateView()
    {
        simulationOptions = new SimulationOptions();
        simulationOptions.StartButton.clicked += StartButtonOnClicked;
        simulationOptions.ResetButton.clicked += ResetButtonOnClicked;
        simulationOptions.StepBackButton.clicked += StepBackButtonOnClicked;
        simulationOptions.StepForwardButton.clicked += StepForwardButtonOnClicked;

        Add(simulationOptions);

        graphViewChanged += OnGraphViewChanged;

        forceDirectedGraphManipulator = new ForceDirectedGraphManipulator();

        this.AddManipulator(forceDirectedGraphManipulator);

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);
        RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
    }

    private void StartButtonOnClicked()
    {
        ClearGraph();
        simulationData.InitSimulation();
        UpdateGraph();
    }

    private void ResetButtonOnClicked()
    {
        ClearGraph();
        simulationData.InitSimulation();
        UpdateGraph();
    }

    private void StepBackButtonOnClicked()
    {
        simulationData.GoToStep(simulationData.CurrentStep - 1);
        UpdateGraph();
    }

    private void StepForwardButtonOnClicked()
    {
        simulationData.GoToStep(simulationData.CurrentStep + 1);
        UpdateGraph();
    }

    private void UpdateGraph()
    {
        simulationOptions.StepSlider.highValue = simulationData.Steps.Count - 1;

        simulationSO.Update();

        SerializedProperty stepsProperty = simulationSO.FindProperty(
            GUIUtils.GetBackingFieldName(nameof(EditorSimulationData.Steps)));

        SerializedProperty stepProperty =
            stepsProperty.GetArrayElementAtIndex(simulationData.CurrentStep);

        LoadGraph(stepProperty);
    }

    private void OnKeyUpEvent(KeyUpEvent evt)
    {
        if (evt.keyCode == KeyCode.Space)
        {
            // ToDo : step forward

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                if (element is GraphNode n)
                {
                    graphViewChange.elementsToRemove =
                        graphViewChange.elementsToRemove.Union(n.Edges).ToList();
                }
            }

            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                if (element is DirectEdge e)
                {
                    e.startNode.Edges.Remove(e);
                    e.endNode.Edges.Remove(e);
                }
            }
        }

        return graphViewChange;
    }

    private GraphNode AddNode(Vector2 position, string id = "", string symbol = "")
    {
        string uniqueID = FindUniqueNodeID(id);

        int index = nodesProperty.arraySize;
        nodesProperty.InsertArrayElementAtIndex(index);
        SerializedProperty newNodeProperty = nodesProperty.GetArrayElementAtIndex(index);

        newNodeProperty.serializedObject.ApplyModifiedProperties();

        GraphNode newNode = AddNode(newNodeProperty);

        newNode.ID = uniqueID;
        newNode.Symbol = symbol;
        newNode.Position = position;

        EditorApplication.delayCall +=
            () => newNode.SetPosition(new Rect(position, Vector2.zero));

        return newNode;
    }

    private GraphNode AddNode(string id = "", string symbol = "")
    {
        return AddNode(Vector2.zero, id, symbol);
    }

    private GraphNode AddNode(SerializedProperty nodeData)
    {
        var newNode = new GraphNode(nodeData);

        newNode.RegisterCallback<MouseUpEvent>(OnMouseUp);
        newNode.RegisterCallback<MouseDownEvent>(OnMouseDown);

        AddElement(newNode);

        return newNode;
    }

    private DirectEdge AddEdge(SerializedProperty edgeData)
    {
        SerializedProperty sourceNodeProperty =
            edgeData.FindPropertyRelative(nameof(GraphEdgeData.source));
        SerializedProperty targetNodeProperty =
            edgeData.FindPropertyRelative(nameof(GraphEdgeData.target));

        GraphNode nodeSource = this.Q<GraphNode>(sourceNodeProperty.stringValue);
        GraphNode nodeTarget = this.Q<GraphNode>(targetNodeProperty.stringValue);

        if (!CanCreateEdge(nodeSource, nodeTarget))
            return null;

        var nodeEdge = new DirectEdge(nodeSource, nodeTarget, edgeData);

        if (!nodeSource.Edges.Contains(nodeEdge))
            nodeSource.Edges.Add(nodeEdge);
        if (!nodeTarget.Edges.Contains(nodeEdge))
            nodeTarget.Edges.Add(nodeEdge);

        AddElement(nodeEdge);

        return nodeEdge;
    }

    private DirectEdge AddEdge(GraphNode nodeSource, GraphNode nodeTarget, string id = "")
    {
        if (!CanCreateEdge(nodeSource, nodeTarget))
            return null;

        string uniqueID = FindUniqueEdgeID(id);

        int index = edgesProperty.arraySize;
        edgesProperty.InsertArrayElementAtIndex(index);
        SerializedProperty newEdgeProperty = edgesProperty.GetArrayElementAtIndex(index);

        newEdgeProperty.serializedObject.ApplyModifiedProperties();

        DirectEdge newEdge = AddEdge(newEdgeProperty);
        newEdge.ID = uniqueID;

        return newEdge;
    }

    private bool CanCreateEdge(GraphNode nodeSource, GraphNode nodeTarget)
    {
        return nodeSource != null &&
               nodeTarget != null &&
               nodeSource != nodeTarget &&
               !edges.Cast<DirectEdge>().
                      Any(directEdge =>
                              (directEdge.startNode == nodeSource && directEdge.endNode == nodeTarget) ||
                              (directEdge.startNode == nodeTarget && directEdge.endNode == nodeSource));
    }

    private string FindUniqueEdgeID(string id = "")
    {
        bool IsUnique(string id) => this.Q<DirectEdge>(id) == null;

        if (!string.IsNullOrEmpty(id))
        {
            if (IsUnique(id))
                return id;
        }

        id ??= "";
        var uniqueID = 0;

        while (true)
        {
            if (IsUnique(id + uniqueID))
                return id + uniqueID;

            uniqueID++;
        }
    }

    private string FindUniqueNodeID(string id = "")
    {
        bool IsUnique(string id) => this.Q<GraphNode>(id) == null;

        if (!string.IsNullOrEmpty(id))
        {
            if (IsUnique(id))
                return id;
        }

        id ??= "";
        var uniqueID = 0;

        while (true)
        {
            if (IsUnique(id + uniqueID))
                return id + uniqueID;

            uniqueID++;
        }
    }

    public void Setup(SerializedObject genDataSO)
    {
        this.genDataSO = genDataSO;
        simulationData = ScriptableObject.CreateInstance<EditorSimulationData>();
        simulationData.GenerationData = genDataSO.targetObject as GenerationData;
        simulationSO = new SerializedObject(simulationData);

        SerializedProperty seedProperty = simulationSO.FindProperty(
            GUIUtils.GetBackingFieldName(nameof(EditorSimulationData.Seed)));
        simulationOptions.SeedField.BindProperty(seedProperty);
        simulationOptions.SeedField.Bind(simulationSO);

        SerializedProperty currentStepProperty = simulationSO.FindProperty(
            GUIUtils.GetBackingFieldName(nameof(EditorSimulationData.CurrentStep)));
        simulationOptions.StepSlider.BindProperty(currentStepProperty);
        simulationOptions.StepSlider.Bind(simulationSO);
    }


    public void LoadGraph(SerializedProperty graphDataSP)
    {
        //graphElements.ForEach(RemoveElement);

        SerializedProperty graphDataSO = graphDataSP;
        nodesProperty = graphDataSP.FindPropertyRelative(nameof(GraphData.nodes));
        edgesProperty = graphDataSP.FindPropertyRelative(nameof(GraphData.edges));

        if (GenData != null)
        {
            // Load nodes
            var removeGraphNodes = this.Query<GraphNode>().ToList();

            for (var i = 0; i < nodesProperty.arraySize; i++)
            {
                SerializedProperty nodeProperty = nodesProperty.GetArrayElementAtIndex(i);
                SerializedProperty idProperty = nodeProperty.FindPropertyRelative(nameof(GraphNodeData.id));
                SerializedProperty symbolProperty = nodeProperty.FindPropertyRelative(nameof(GraphNodeData.symbol));

                int index = removeGraphNodes.FindIndex(
                    _ => _.ID == idProperty.stringValue &&
                         _.Symbol == symbolProperty.stringValue);
                if (index >= 0)
                    removeGraphNodes.RemoveAt(index);
                else
                    AddNode(nodeProperty);
            }

            removeGraphNodes.ForEach(RemoveElement);

            // Load edges

            edges.ForEach(RemoveElement);

            for (var i = 0; i < edgesProperty.arraySize; i++)
            {
                SerializedProperty edgeProperty = edgesProperty.GetArrayElementAtIndex(i);
                AddEdge(edgeProperty);
            }

            //EditorApplication.delayCall += () => FrameAll();
        }
    }

    public void UpdatePositions()
    {
        forceDirectedGraphManipulator.Update();

        foreach (GraphElement edge in graphElements)
        {
            if (edge is DirectEdge de)
                de.UpdatePositionAndRotation();
        }
    }
}
