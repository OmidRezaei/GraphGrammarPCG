using System.Linq;
using CommonEditorElements.Graph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphRewriteView : CommonGraphView
{
    public GraphData graphData;
    private ForceDirectedGraphManipulator forceDirectedGraphManipulator;
    private bool startDrag;
    private GraphNode startDragNode;
    private GraphNode endDragNode;

    private SerializedProperty graphDataSO;
    private SerializedProperty nodesProperty;
    private SerializedProperty edgesProperty;

    public GraphRewriteView()
    {
        graphViewChanged += OnGraphViewChanged;
        graphViewChanged += OnGraphViewChangedDeleteProperty;

        forceDirectedGraphManipulator = new ForceDirectedGraphManipulator();

        this.AddManipulator(forceDirectedGraphManipulator);

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);
        RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
    }

    private void OnKeyUpEvent(KeyUpEvent evt)
    {
        if (evt.keyCode is KeyCode.LeftShift or KeyCode.RightShift)
        {
            startDrag = false;
            evt.StopPropagation();
        }
        else if (evt.keyCode == KeyCode.Space)
        {
            Vector2 mp = evt.imguiEvent.mousePosition;
            Vector2 localPos =
                (evt.currentTarget as VisualElement).
                ChangeCoordinatesTo(contentViewContainer, mp);

            AddNode(localPos);

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (evt.button == 2)
        {
        }
        else if (evt.shiftKey)
        {
            if (evt.target is VisualElement ve)
            {
                GraphNode gn = ve.GetFirstAncestorOfType<GraphNode>();

                if (gn != null && startDrag)
                {
                    endDragNode = gn;

                    if (CanCreateEdge(startDragNode, endDragNode))
                    {
                        AddEdge(startDragNode, endDragNode);

                        evt.StopPropagation();
                    }

                    endDragNode = null;
                    startDragNode = null;
                    startDrag = false;
                }
            }
        }
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.shiftKey)
        {
            if (evt.target is VisualElement ve)
            {
                GraphNode gn = ve.GetFirstAncestorOfType<GraphNode>();

                if (gn != null)
                {
                    startDrag = true;
                    startDragNode = gn;

                    evt.StopPropagation();
                }
            }
        }
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
                    e.DeleteProperty();
                }
            }
        }

        return graphViewChange;
    }

    private GraphViewChange OnGraphViewChangedDeleteProperty(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                if (element is GraphNode n)
                    n.DeleteProperty();
                else if (element is DirectEdge e)
                    e.DeleteProperty();
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

    private DirectEdge AddEdge(GraphNode nodeSource, GraphNode nodeTarget, SerializedProperty edgeData)
    {
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

        DirectEdge newEdge = AddEdge(nodeSource, nodeTarget, newEdgeProperty);
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

    public void LoadGraph(SerializedProperty graphDataSO)
    {
        this.graphDataSO = graphDataSO;
        this.TrackPropertyValue(graphDataSO, OnGraphDataChanged);
        graphData = graphDataSO.boxedValue as GraphData;
        nodesProperty = graphDataSO.FindPropertyRelative(nameof(GraphData.nodes));
        edgesProperty = graphDataSO.FindPropertyRelative(nameof(GraphData.edges));

        if (graphData != null)
        {
            // Load nodes
            for (var i = 0; i < nodesProperty.arraySize; i++)
            {
                SerializedProperty nodeProperty = nodesProperty.GetArrayElementAtIndex(i);
                AddNode(nodeProperty);
            }

            // Load edges
            for (var i = 0; i < edgesProperty.arraySize; i++)
            {
                SerializedProperty edgeProperty = edgesProperty.GetArrayElementAtIndex(i);
                SerializedProperty sourceNodeProperty = edgeProperty.FindPropertyRelative(nameof(GraphEdgeData.source));
                SerializedProperty targetNodeProperty = edgeProperty.FindPropertyRelative(nameof(GraphEdgeData.target));

                AddEdge(this.Q<GraphNode>(sourceNodeProperty.stringValue),
                        this.Q<GraphNode>(targetNodeProperty.stringValue),
                        edgeProperty);
            }

            EditorApplication.delayCall += () => FrameAll();
        }
    }

    private void OnGraphDataChanged(SerializedProperty sp)
    {
        Debug.Log(sp);
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
