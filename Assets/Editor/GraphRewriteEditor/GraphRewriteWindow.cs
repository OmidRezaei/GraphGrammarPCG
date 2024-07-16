using EditorExtensions;
using UnityEditor;
using UnityEngine.UIElements;

public class GraphRewriteWindow : EditorWindow
{
    private VisualElement mainPanel;

    private GenerationData genData;
    private SerializedObject genDataSerializedObject;
    private GraphRewriteMainPanel graphRewriteMainPanel;

    [MenuItem("Window/Graph Rewrite Window")]
    public static void ShowWindow()
    {
        GetWindow<GraphRewriteWindow>("Graph Rewrite Window");
    }

    [MenuItem("Window/Force Close Graph Rewrite Window")]
    public static void ForceClose()
    {
        if (HasOpenInstances<GraphRewriteWindow>())
            GetWindow<GraphRewriteWindow>().Close();
    }

    private void CreateGUI()
    {
        rootVisualElement.AddUXMLByName<GraphRewriteWindow>();

        mainPanel = new VisualElement
        {
            style =
            {
                flexGrow = 1
            }
        };

        rootVisualElement.Add(mainPanel);

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if (Selection.count == 1 &&
            Selection.activeObject &&
            Selection.activeObject is GenerationData gd)
        {
            genData = gd;
            genDataSerializedObject = new SerializedObject(genData);

            graphRewriteMainPanel = new GraphRewriteMainPanel
            {
                style =
                {
                    flexGrow = 1
                }
            };

            graphRewriteMainPanel.LoadGraph(genDataSerializedObject);
            mainPanel.Add(graphRewriteMainPanel);
        }
        else
        {
            genData = null;
            genDataSerializedObject = null;
            mainPanel.Clear();
        }
    }

    private void Update()
    {
        graphRewriteMainPanel?.Update();
    }
}
