﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GenGra
{
    public class GenGraParser : MonoBehaviour
    {
        [SerializeField] private string missionGraphGrammarFilePath;

        /*
         * See doc comment on struct SpaceObjectByMissionSymbol about the
         * necessity for both a public Array and a private IDictionary
         */
        [SerializeField] private SpaceObjectByMissionSymbol[] spaceObjectsByMissionSymbol;
        private IDictionary<string, GameObject> spaceObjects;

        private IDictionary<string, GameObject> SpaceObjects
        {
            get
            {
                if (spaceObjects == null)
                {
                    spaceObjects = new Dictionary<string, GameObject>();
                    foreach (SpaceObjectByMissionSymbol spaceObjectByMissionSymbol in spaceObjectsByMissionSymbol)
                    {
                        spaceObjects[spaceObjectByMissionSymbol.MissionSymbol] =
                            spaceObjectByMissionSymbol.SpaceObject;
                    }
                }

                return spaceObjects;
            }
        }

        void Start()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            long timeBeforeXmlDeserialization = stopwatch.ElapsedMilliseconds;

            GenGraType genGra;
            using (FileStream fileStream = new FileStream(missionGraphGrammarFilePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GenGraType));
                genGra = (GenGraType) serializer.Deserialize(fileStream);
            }

            long timeAfterXmlDeserialization = stopwatch.ElapsedMilliseconds;
            long xmlDeserializationTime = timeAfterXmlDeserialization - timeBeforeXmlDeserialization;
            Debug.Log($"XML deserialization completed in: {xmlDeserializationTime}ms");

            DebugLogDeserialization(genGra);

            long timeBeforeGraphTransformation = stopwatch.ElapsedMilliseconds;

            GraphType missionGraph = TransformGraph(genGra);

            long timeAfterGraphTransformation = stopwatch.ElapsedMilliseconds;
            long graphTransformationTime = timeAfterGraphTransformation - timeBeforeGraphTransformation;
            Debug.Log($"Mission graph generation completed in: {graphTransformationTime}ms");
            DebugLogGraph(missionGraph);

            long timeBeforeSpaceGeneration = stopwatch.ElapsedMilliseconds;

            GenerateSpace(missionGraph);

            long timeAfterSpaceGeneration = stopwatch.ElapsedMilliseconds;
            long spaceGenerationTime = timeAfterSpaceGeneration - timeBeforeSpaceGeneration;
            Debug.Log($"Space generation completed in: {spaceGenerationTime}ms");

            stopwatch.Stop();
            Debug.Log($"Total execution completed in: {stopwatch.ElapsedMilliseconds}ms");
        }

        // TODO: move this method to be an instance method on GenGraType
        private GraphType TransformGraph(GenGraType genGra)
        {
            IDictionary<string, GraphType> graphs = new Dictionary<string, GraphType>(genGra.Graphs.Graph.Length);
            foreach (GraphType graph in genGra.Graphs.Graph)
            {
                graphs[graph.id] = graph;
            }

            string startGraphRef = genGra.Grammar.StartGraph.@ref;
            GraphType startGraph = graphs[startGraphRef];

            int ruleNumber = 0;

            while (true)
            {
                RuleType[] applicableRules = GetApplicableRules(genGra, graphs, startGraph);
                if (applicableRules.Length == 0) return startGraph;

                RuleType ruleToApply = applicableRules.Length == 1
                    ? applicableRules[0]
                    : applicableRules[Random.Range(0, applicableRules.Length - 1)];

                Debug.Log(
                    $"[Applying Rule {++ruleNumber}] source: {ruleToApply.source} | target: {ruleToApply.target}");

                GraphType ruleSource = graphs[ruleToApply.source];
                GraphType ruleTarget = graphs[ruleToApply.target];
                startGraph.FindAndReplace(ruleSource, ruleTarget);
            }
        }

        // TODO: move this method to be an instance method on GenGraType
        private static RuleType[] GetApplicableRules(GenGraType genGra, IDictionary<string, GraphType> graphs,
            GraphType startGraph)
        {
            return genGra.Grammar.Rules.Rule
                .Where(rule =>
                {
                    GraphType ruleSourceGraph = graphs[rule.source];
                    return startGraph.IsSupergraphOf(ruleSourceGraph);
                })
                .ToArray();
        }

        // TODO: remove this method when done prototyping
        private void DebugLogDeserialization(GenGraType genGra)
        {
            foreach (GraphType graph in genGra.Graphs.Graph)
            {
                DebugLogGraph(graph);
            }

            Debug.Log($"[Grammar] StartGraph: {genGra.Grammar.StartGraph.@ref}");
            RuleType[] rules = genGra.Grammar.Rules.Rule;
            for (int i = 0; i < rules.Length; i++)
            {
                RuleType rule = rules[i];
                Debug.Log($"[Grammar | Rule {i + 1}] source: {rule.source} | target: {rule.target}");
            }
        }

        // TODO: remove this method when done prototyping
        private void DebugLogGraph(GraphType graph)
        {
            foreach (NodeType node in graph.Nodes.Node)
            {
                Debug.Log($"[Graph {graph.id} | Node: {node.id}] symbol: {node.symbol}");
            }

            if (graph.Edges == null || graph.Edges.Edge == null) return;

            for (int j = 0; j < graph.Edges.Edge.Length; j++)
            {
                EdgeType edge = graph.Edges.Edge[j];
                Debug.Log($"[Graph {graph.id} | Edge: {j + 1}] source: {edge.source} | target: {edge.target}");
            }
        }

        private void GenerateSpace(GraphType missionGraph)
        {
            // Carry out a BFS through missionGraph. For each node, retrieve the correct SpaceObjectByMissionSymbol
            // based on the node's symbol and place the SpaceObject on the plane of the scene, connecting it to
            // an existing SpaceObject where possible (i.e. not the first SpaceObject placed, and only connected where
            // specified by the SpaceObject - such as a doorway).
            foreach (NodeType startNode in missionGraph.StartNodes)
            {
                BFS(missionGraph, startNode);
            }
        }

        private void BFS(GraphType graph, NodeType startNode)
        {
            IList<string> visitedNodeIds = new List<string>(graph.Nodes.Node.Length);
            Queue<NodeType> queue = new Queue<NodeType>();

            visitedNodeIds.Add(startNode.id);
            queue.Enqueue(startNode);

            while (queue.Count != 0)
            {
                NodeType currentNode = queue.Dequeue();
                IList<NodeType> adjacentNodes = graph.AdjacencyList[currentNode.id];
                foreach (NodeType adjacentNode in adjacentNodes)
                {
                    if (visitedNodeIds.Contains(adjacentNode.id)) continue;
                    visitedNodeIds.Add(adjacentNode.id);
                    PlaceSpaceObject(adjacentNode.symbol);
                    queue.Enqueue(adjacentNode);
                }
            }
        }

        private void PlaceSpaceObject(string missionSymbol)
        {
            GameObject spaceObject = SpaceObjects[missionSymbol];
            Debug.Log($"[Place Space Object] Mission Symbol: {missionSymbol} | Space Object: {spaceObject}");
            // place spaceObject on plane in scene, relative to other spaceObjects
        }

        /**
         * This struct is a workaround to allow GameObjects to be mapped to a string in the Unity editor in a
         * similar vein to using a Dictionary, because IDictionary is not serializable by the Unity engine.
         */
        [Serializable]
        private struct SpaceObjectByMissionSymbol
        {
            [SerializeField] private string missionSymbol;
            [SerializeField] private GameObject spaceObject;

            public string MissionSymbol => missionSymbol;

            public GameObject SpaceObject => spaceObject;
        }
    }
}