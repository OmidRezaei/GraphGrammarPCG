using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ForceDirectedGraphManipulator : Manipulator
{
    private GraphView graph;

    private Dictionary<GraphNode, Vector2> velocity;

    protected override void RegisterCallbacksOnTarget()
    {
        graph = target as GraphView;
        velocity = new Dictionary<GraphNode, Vector2>();
    }

    protected override void UnregisterCallbacksFromTarget()
    {
    }

    public void Update()
    {
        const float repulsionForce = 7500f; // Repulsive force between nodes
        const float attractionForce = 0.02f; // Attractive force of edges
        const float damping = 0.9f;        // Damping to slow down the motion

        var nodes = graph.Query<GraphNode>().ToList();
        var edges = graph.Query<DirectEdge>().ToList();


        //for (var i = 0; i < 100; i++) // Run the algorithm for 100 iterations
        {
            // Calculate repulsion forces between nodes
            foreach (GraphNode nodeA in nodes)
            {
                Vector2 force = Vector2.zero;

                foreach (GraphNode nodeB in nodes)
                {
                    if (nodeA == nodeB)
                        continue;

                    Vector2 delta = nodeA.layout.position - nodeB.layout.position;
                    float distance = delta.magnitude;

                    if (distance >= 0)
                    {
                        distance += 1;
                        Vector2 repulsion = delta.normalized * repulsionForce / (distance * distance);
                        force += repulsion;
                    }
                }

                if (!velocity.ContainsKey(nodeA))
                    velocity.Add(nodeA, Vector2.zero);

                if (graph.selection.Contains(nodeA))
                    velocity[nodeA] = Vector2.zero;
                else
                    velocity[nodeA] = force;
            }

            // Calculate attractive forces for edges
            foreach (DirectEdge edge in edges)
            {
                if (edge.startNode == null || edge.endNode == null)
                    continue;

                Vector2 delta = edge.startNode.layout.position - edge.endNode.layout.position;
                float distance = delta.magnitude;
                Vector2 force =
                    attractionForce * Mathf.Max(0, distance - 100) * delta.normalized; // Desired edge length


                if (!velocity.ContainsKey(edge.startNode))
                    velocity.Add(edge.startNode, Vector2.zero);

                if (!graph.selection.Contains(edge.startNode))
                    velocity[edge.startNode] -= force;

                if (!velocity.ContainsKey(edge.endNode))
                    velocity.Add(edge.endNode, Vector2.zero);

                if (!graph.selection.Contains(edge.endNode))
                    velocity[edge.endNode] += force;
            }

            // Apply velocities to positions
            foreach (GraphNode node in nodes)
            {
                if (velocity.TryGetValue(node, out Vector2 vel))
                {
                    if (vel != Vector2.zero)
                    {
                        Vector2 newPos = node.layout.position + vel * damping;
                        node.SetPosition(new Rect(newPos, Vector2.zero));
                        node.UpdatePresenterPosition();
                    }
                    //node.style.left = newPos.x;
                    //node.style.top = newPos.y;
                }
            }
        }

        // Update edges after positions have been updated
        foreach (DirectEdge edge in edges)
            edge.UpdatePositionAndRotation();
    }
}
