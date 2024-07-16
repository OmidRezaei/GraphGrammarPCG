using System.Linq;
using GenGra;
using UnityEngine;

public static class GraphTransformer
{
    public static GenGraType ToGenGra(this GenerationData generationData)
    {
        var gg = new GenGraType
        {
            Graphs = new GraphsType
            {
                Graph = generationData.Graphs.Select(g => new GraphType
                                       {
                                           id = g.id,
                                           Nodes = new NodesType
                                           {
                                               Node = g.nodes.Select(n => new NodeType
                                                        {
                                                            id = n.id,
                                                            symbol = n.symbol,
                                                            EditorPosition = Random.insideUnitCircle * 50
                                                        }).
                                                        ToArray()
                                           },
                                           Edges = new EdgesType
                                           {
                                               Edge = g.edges.Select(e => new EdgeType
                                                        {
                                                            id = e.id,
                                                            source = e.source,
                                                            target = e.target
                                                        }).
                                                        ToArray()
                                           }
                                       }).
                                       ToArray()
            },
            Grammar = new GrammarType
            {
                StartGraph = new StartGraphType
                {
                    @ref = generationData.StartGraphID
                },
                Rules = new RulesType
                {
                    Rule = generationData.Rules.Select(r => new RuleType
                                          {
                                              id = r.id,
                                              source = r.sourceGraph,
                                              target = r.targetGraph
                                          }).
                                          ToArray()
                }
            }
        };

        return gg;
    }

    public static GraphData ToGraphData(this GraphType graph)
    {
        var gd = new GraphData
        {
            id = graph.id,
            nodes = graph.Nodes.Node.Select(n => new GraphNodeData
                          {
                              editorPosition = n.EditorPosition,
                              id = n.id,
                              symbol = n.symbol
                          }).
                          ToArray(),
            edges = graph.Edges.Edge.Select(e => new GraphEdgeData
                          {
                              id = e.id,
                              source = e.source,
                              target = e.target
                          }).
                          ToArray(),
            StartNodesIDs = graph.StartNodes.Select(sn => sn.id).ToArray()
        };

        return gd;
    }
}
