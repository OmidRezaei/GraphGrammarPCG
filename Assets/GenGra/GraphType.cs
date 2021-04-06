﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GenGra
{
    public partial class GraphType
    {
        private IDictionary<string, IList<NodeType>> adjacencyList;

        private IDictionary<string, IList<NodeType>> nodeSymbolMap;

        private NodeType[] startingNodes;

        public IDictionary<string, IList<NodeType>> AdjacencyList
        {
            get
            {
                if (adjacencyList == null)
                {
                    adjacencyList = new Dictionary<string, IList<NodeType>>();
                    IDictionary<string, NodeType> nodes = new Dictionary<string, NodeType>();
                    
                    foreach (NodeType node in Nodes.Node)
                    {
                        adjacencyList[node.id] = new List<NodeType>();
                        nodes[node.id] = node;
                    }
                    foreach (EdgeType edge in Edges.Edge)
                    {
                        NodeType targetNode = nodes[edge.target];
                        adjacencyList[edge.source].Add(targetNode);
                    }
                }
                
                return adjacencyList;
            }
        }

        public IDictionary<string, IList<NodeType>> NodeSymbolMap
        {
            get
            {
                if (nodeSymbolMap == null)
                {
                    nodeSymbolMap = new Dictionary<string, IList<NodeType>>();
                    foreach (NodeType node in Nodes.Node)
                    {
                        if (!nodeSymbolMap.ContainsKey(node.symbol))
                        {
                            nodeSymbolMap[node.symbol] = new List<NodeType>();
                        }
                        nodeSymbolMap[node.symbol].Add(node);
                    }
                }

                return nodeSymbolMap;
            }
        }

        public NodeType[] StartingNodes
        {
            get
            {
                if (startingNodes != null) return startingNodes;
                
                // If graph has no edges then nodes in graph are disconnected,
                // therefore all nodes should be used as starting nodes
                if (Edges.Edge.Length == 0)
                {
                    if (Nodes.Node.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "Graph has no nodes or edges. Please check the validity of your grammar");
                    }
                    startingNodes = Nodes.Node;
                    return startingNodes;
                }

                // Calculate the indegree for each node in the graph
                IDictionary<string, int> nodeIndegrees = new Dictionary<string, int>();
                foreach (EdgeType edge in Edges.Edge)
                {
                    if (!nodeIndegrees.ContainsKey(edge.target))
                    {
                        nodeIndegrees[edge.target] = 0;
                    }
                    nodeIndegrees[edge.target]++;
                }

                // Find all nodes with an indegree of 0
                NodeType[] returnNodes = Nodes.Node
                    .Where(node => !nodeIndegrees.ContainsKey(node.id))
                    .ToArray();

                if (returnNodes.Length != 0)
                {
                    startingNodes = returnNodes;
                    return startingNodes;
                }

                // If no node exists with an indegree of 0, then this graph is cyclic, so set startingNodes
                // to be an array containing just one random start node, as the actual start node will not
                // matter since all nodes can be visited from any other node
                NodeType randomNode = Nodes.Node[Random.Range(0, Nodes.Node.Length - 1)];
                startingNodes = new [] {randomNode};
                return startingNodes;
            }
        }

        public bool IsSupergraphOf(GraphType otherGraph)
        {
            if (!HasAllSymbolsIn(otherGraph)) return false;

            foreach (NodeType startingNode in otherGraph.StartingNodes)
            {
                IList<NodeType> otherNodes = new List<NodeType>(otherGraph.AdjacencyList[startingNode.id]);
                otherNodes.Add(startingNode);
                
                bool isSuccessfulCandidate = false;
                IList<NodeType> nodeCandidates = NodeSymbolMap[startingNode.symbol];
                foreach (NodeType nodeCandidate in nodeCandidates)
                {
                    IList<NodeType> thisNodes = new List<NodeType>(AdjacencyList[nodeCandidate.id]);
                    thisNodes.Add(nodeCandidate);
                    
                    isSuccessfulCandidate = DualSearch(otherGraph, thisNodes, otherNodes);
                    if (isSuccessfulCandidate) break;
                }
                if (!isSuccessfulCandidate) return false;
            }
            
            return true;
        }

        private bool HasAllSymbolsIn(GraphType otherGraph)
        {
            return otherGraph.NodeSymbolMap.All(pair =>
            {
                string symbol = pair.Key;
                IList<NodeType> otherGraphNodes = pair.Value;
                return NodeSymbolMap.ContainsKey(symbol) && 
                       NodeSymbolMap[symbol].Count >= otherGraphNodes.Count;
            });
        }

        private bool DualSearch(GraphType otherGraph, IList<NodeType> thisNodes, 
            IList<NodeType> otherNodes, ISet<string> visitedOtherNodes = null,
            IDictionary<string, NodeType> nodesMarkedByOtherNodeId = null)
        {
            visitedOtherNodes = visitedOtherNodes ?? new HashSet<string>();
            
            foreach (NodeType otherNode in otherNodes)
            {
                if (visitedOtherNodes.Contains(otherNode.id)) continue;
                visitedOtherNodes.Add(otherNode.id);
                
                bool matchingNodeFound = false;
                foreach (NodeType thisNode in thisNodes)
                {
                    if (thisNode.symbol == otherNode.symbol)
                    {
                        IList<NodeType> thisAdjacentNodes = AdjacencyList[thisNode.id];
                        IList<NodeType> otherAdjacentNodes = otherGraph.AdjacencyList[otherNode.id];
                        matchingNodeFound = DualSearch(otherGraph, thisAdjacentNodes, otherAdjacentNodes, visitedOtherNodes, nodesMarkedByOtherNodeId);
                        if (matchingNodeFound)
                        {
                            if (nodesMarkedByOtherNodeId != null)
                            {
                                nodesMarkedByOtherNodeId[otherNode.id] = thisNode;
                            }
                            break;
                        }
                    }
                }
                if (!matchingNodeFound) return false;
            }

            return true;
        }

        public void FindAndReplace(GraphType source, GraphType target)
        {
            IDictionary<string, NodeType> nodesMarkedBySourceNodeId = new Dictionary<string, NodeType>();
            
            foreach (NodeType startingNode in source.StartingNodes)
            {
                IList<NodeType> sourceNodes = new List<NodeType>(source.AdjacencyList[startingNode.id]);
                sourceNodes.Add(startingNode);

                bool isSuccessfulCandidate = false;
                IList<NodeType> nodeCandidates = NodeSymbolMap[startingNode.symbol];
                foreach (NodeType nodeCandidate in nodeCandidates)
                {
                    IList<NodeType> thisNodes = new List<NodeType>(AdjacencyList[nodeCandidate.id]);
                    thisNodes.Add(nodeCandidate);
                    
                    isSuccessfulCandidate = DualSearch(source, thisNodes, sourceNodes, nodesMarkedBySourceNodeId);
                    if (isSuccessfulCandidate) break;
                    nodesMarkedBySourceNodeId = new Dictionary<string, NodeType>();
                }

                if (!isSuccessfulCandidate)
                {
                    throw new InvalidOperationException($"No subgraph found in graph {id} matching source graph" +
                                                        $" {source.id}, so cannot carry out find and replace operation");
                }
            }

            foreach (var kvp in nodesMarkedBySourceNodeId)
            {
                string sourceNodeId = kvp.Key;
                NodeType node = kvp.Value;
                Debug.Log($"[Marked node] Source Node ID: {sourceNodeId} | This Node ID: {node.id}");
            }
        }

        private bool DualSearch(GraphType otherGraph, IList<NodeType> thisNodes,
            IList<NodeType> otherNodes, IDictionary<string, NodeType> nodesMarkedByOtherNodeId)
        {
            return DualSearch(otherGraph, thisNodes, otherNodes, null, nodesMarkedByOtherNodeId);
        }
    }
}