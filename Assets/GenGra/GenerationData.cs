using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationData : ScriptableObject
{
    [field: SerializeField]
    public List<GraphData> Graphs { get; set; }

    [field: SerializeField]
    public string StartGraphID { get; set; }

    [field: SerializeField]
    public List<RuleData> Rules { get; set; }

    [field: SerializeField]
    public int Seed { get; set; } = -1;
}

[Serializable]
public struct RuleData
{
    public string id;

    public string sourceGraph;
    public string targetGraph;
}

[Serializable]
public class GraphData
{
    public string id;

    public GraphNodeData[] nodes;
    public GraphEdgeData[] edges;
    public string[] StartNodesIDs;
}

[Serializable]
public struct GraphNodeData
{
    public string id;
    public string symbol;

    [HideInInspector]
    [SerializeField]
    public Vector2 editorPosition;
}

[Serializable]
public struct GraphEdgeData
{
    public string id;
    public string source; // id of the source node
    public string target; // id of the target node
}
