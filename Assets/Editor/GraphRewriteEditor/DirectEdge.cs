using EditorExtensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DirectEdge : Edge
{
    public GraphNode startNode;
    public GraphNode endNode;

    private readonly SerializedProperty edgeData;
    private readonly SerializedProperty idProperty;
    private readonly SerializedProperty sourceGraphProperty;
    private readonly SerializedProperty targetGraphProperty;

    public string ID
    {
        get => idProperty.stringValue;
        set
        {
            idProperty.stringValue = value;
            idProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    public DirectEdge(GraphNode startNode, GraphNode endNode, SerializedProperty edgeData)
    {
        this.AddUXMLByName();

        this.edgeData = edgeData;
        idProperty = edgeData.FindPropertyRelative(nameof(GraphEdgeData.id));
        sourceGraphProperty = edgeData.FindPropertyRelative(nameof(GraphEdgeData.source));
        sourceGraphProperty.stringValue = startNode.ID;
        targetGraphProperty = edgeData.FindPropertyRelative(nameof(GraphEdgeData.target));
        targetGraphProperty.stringValue = endNode.ID;

        name = ID;

        this.startNode = startNode;
        this.endNode = endNode;

        style.position = Position.Absolute;

        layer = -1;
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public void UpdatePositionAndRotation()
    {
        style.transformOrigin =
            new StyleTransformOrigin(new TransformOrigin(0, 0));

        Vector2 startPosition =
            startNode.layout.position +
            new Vector2(startNode.layout.width / 2, startNode.layout.height / 2);

        Vector2 endPosition =
            endNode.layout.position +
            new Vector2(endNode.layout.width / 2, endNode.layout.height / 2);

        float angleRad = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x);
        float angle = angleRad * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = Vector2.Distance(startPosition, endPosition);
        style.width = distance;
        //style.width = Mathf.Abs(Mathf.Cos(angleRad) * distance);
        //style.height = Mathf.Abs(Mathf.Sin(angleRad) * distance);

        Vector2 midPoint = (startPosition + endPosition) / 2;
        style.left = startPosition.x;
        style.top = startPosition.y;
    }

    public void DeleteProperty()
    {
        edgeData.DeleteCommand();
    }
}
