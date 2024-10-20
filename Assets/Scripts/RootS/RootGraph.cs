using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootGraph
    {
        private List<Node> nodes = new List<Node>();


        public Node AddRootNode(Vector3 position)
        {
            nodes.Add(new Node(position));
            return nodes[nodes.Count - 1];
        }
        public Node AddNode(Vector3 position, Node parent)
        {
            Node newNode = new Node(position, parent);
            parent.Childrens.Add(newNode);
            nodes.Add(newNode);
            return newNode;
        }
        public bool isNodeLast(Node node)
        {
            if(node.Childrens.Count == 0)
                return true;
            else 
                return false;
        }
        public void AddEdge(Node nodeA, Node nodeB)
        {
            Edge newEdge = new Edge(nodeA, nodeB);
            //nodeA.edges.Add(newEdge);
            //nodeB.edges.Add(newEdge);
        }
        public Node FindClosestNode(Vector3 position, float radius)
        {
            Node closestNode = null;
            float closestDistance = radius;

            foreach (Node node in nodes)
            {
                float distance = Vector3.Distance(position, node.Position);
                if (distance < closestDistance)
                {
                    closestNode = node;
                    closestDistance = distance;
                }
            }

            return closestNode;
        }

    }
}
