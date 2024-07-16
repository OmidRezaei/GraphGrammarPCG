using EditorExtensions;
using UnityEditor;
using UnityEngine.UIElements;

public class RuleGraphPairView : VisualElement
{
    private SerializedObject genDataSO;
    private VisualElement leftPanel;
    private VisualElement rightPanel;
    private GraphRewriteView leftGraph;
    private GraphRewriteView rightGraph;
    private GenerationData GenData => genDataSO.targetObject as GenerationData;

    public RuleGraphPairView()
    {
        this.AddUXMLByName();

        leftPanel = this.Q<VisualElement>("left-panel");
        rightPanel = this.Q<VisualElement>("right-panel");
    }

    public void Setup(SerializedObject genData, int ruleIndex)
    {
        genDataSO = genData;

        if (GenData)
        {
            RuleData rule = GenData.Rules[ruleIndex];

            leftGraph = TrySetupGraphInElement(rule.sourceGraph, leftPanel);
            rightGraph = TrySetupGraphInElement(rule.targetGraph, rightPanel);
        }
    }

    private GraphRewriteView TrySetupGraphInElement(string graphID, VisualElement element)
    {
        if (!string.IsNullOrWhiteSpace(graphID))
        {
            int index = GenData.Graphs.FindIndex(_ => _.id == graphID);

            if (index == -1)
                return null;

            SerializedProperty graphSP =
                genDataSO.FindProperty(
                              GUIUtils.GetBackingFieldName(
                                  nameof(GenerationData.Graphs))).
                          GetArrayElementAtIndex(index);

            if (graphSP != null)
            {
                var grv = new GraphRewriteView
                {
                    style =
                    {
                        flexGrow = 1
                    }
                };
                grv.LoadGraph(graphSP);
                element.Add(grv);

                return grv;
            }
        }

        return null;
    }

    public void UpdatePositions()
    {
        leftGraph?.UpdatePositions();
        rightGraph?.UpdatePositions();
    }
}
