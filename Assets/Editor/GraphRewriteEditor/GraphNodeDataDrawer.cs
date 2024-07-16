using EditorExtensions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GraphNodeData))]
public class GraphNodeDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GUIUtils.DrawPropertiesInRow(
            position,
            property.FindPropertyRelative(nameof(GraphNodeData.id)),
            property.FindPropertyRelative(nameof(GraphNodeData.symbol)));

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(GraphEdgeData))]
public class GraphEdgeDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GUIUtils.DrawPropertiesInRow(
            position,
            property.FindPropertyRelative(nameof(GraphEdgeData.id)),
            property.FindPropertyRelative(nameof(GraphEdgeData.source)),
            property.FindPropertyRelative(nameof(GraphEdgeData.target)));

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(RuleData))]
public class GraphRuleDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GUIUtils.DrawPropertiesInRow(
            position,
            property.FindPropertyRelative(nameof(RuleData.id)),
            property.FindPropertyRelative(nameof(RuleData.sourceGraph)),
            property.FindPropertyRelative(nameof(RuleData.targetGraph)));

        EditorGUI.EndProperty();
    }
}