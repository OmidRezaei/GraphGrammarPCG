using System.Collections.Generic;
using EditorExtensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphNode : Node
{
    //private NameField symbolNameField;
    private TextField symbolTextField;
    private readonly SerializedProperty positionProperty;
    private readonly SerializedProperty idProperty;
    private readonly SerializedProperty symbolProperty;
    public SerializedProperty NodeData { get; }

    public List<DirectEdge> Edges { get; }

    public string ID
    {
        get => idProperty.stringValue;
        set
        {
            idProperty.stringValue = value;
            idProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    public string Symbol
    {
        get => symbolProperty.stringValue;
        set
        {
            symbolProperty.stringValue = value;
            symbolProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    public Vector2 Position
    {
        get => positionProperty.vector2Value;
        set
        {
            positionProperty.vector2Value = value;
            positionProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    public GraphNode(SerializedProperty nodeData)
    {
        this.AddUXMLByName();
        this[0].RemoveFromHierarchy();
        this[0].BringToFront();

        NodeData = nodeData;
        positionProperty = nodeData.FindPropertyRelative(nameof(GraphNodeData.editorPosition));
        idProperty = nodeData.FindPropertyRelative(nameof(GraphNodeData.id));
        symbolProperty = nodeData.FindPropertyRelative(nameof(GraphNodeData.symbol));

        Label idLabel = this.Q<Label>("id-label");
        idLabel.RegisterValueChangedCallback(evt => name = evt.newValue);
        idLabel.BindProperty(idProperty);
        idLabel.Bind(idProperty.serializedObject);

        symbolTextField = this.Q<TextField>("symbol-textfield");
        symbolTextField.BindProperty(symbolProperty);
        symbolTextField.Bind(symbolProperty.serializedObject);

        //symbolNameField = this.Q<NameField>("symbol-namefield");
        //symbolNameField.SetName(Symbol);
        //symbolNameField.onRename += s => Symbol = s;

        SetPosition(new Rect(Position, Vector2.zero));

        name = ID;

        Edges = new List<DirectEdge>();
    }

    public override void UpdatePresenterPosition()
    {
        Position = layout.position;
    }

    //public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    //{
    //    base.BuildContextualMenu(evt);

    //    evt.menu.AppendAction("Change Symbol", Action);
    //}

    //private void Action(DropdownMenuAction obj)
    //{
    //    symbolNameField.EnterEditMode();
    //}
    public void DeleteProperty()
    {
        NodeData.DeleteCommand();
    }
}
