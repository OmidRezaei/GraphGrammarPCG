using System.Collections.Generic;
using UnityEngine;

namespace GenGra
{
    public partial class GenGraType
    {
        public IEnumerator<GraphType> GenerationGraphStepByStep()
        {
            IDictionary<string, GraphType> graphs = new Dictionary<string, GraphType>(Graphs.Graph.Length);
            foreach (GraphType graph in Graphs.Graph)
                graphs[graph.id] = graph;

            string startGraphRef = Grammar.StartGraph.@ref;
            GraphType startGraph = graphs[startGraphRef];

            yield return startGraph;

            var ruleNumber = 0;

            while (true)
            {
                RuleType[] applicableRules = GetApplicableRules(graphs, startGraph);

                if (applicableRules.Length == 0)
                {
                    yield return startGraph;

                    yield break;
                }

                RuleType ruleToApply = applicableRules[
                    applicableRules.Length == 1 ? 0 : Random.Range(0, applicableRules.Length)];

                Debug.Log(
                    $"[Applying Rule {++ruleNumber}] source: {ruleToApply.source} | target: {ruleToApply.target}");

                GraphType ruleSource = graphs[ruleToApply.source];
                GraphType ruleTarget = graphs[ruleToApply.target];
                startGraph.FindAndReplace(ruleSource, ruleTarget);

                yield return startGraph;
            }
        }
    }
}
