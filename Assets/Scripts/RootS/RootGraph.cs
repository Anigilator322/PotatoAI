using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootGraph
    {
        private List<RootNode> nodes = new List<RootNode>();


        public RootNode AddRootNode(Vector3 position)
        {
            nodes.Add(new RootNode(position));
            return nodes[nodes.Count - 1];
        }
        public RootNode AddNode(Vector3 position, RootNode parent)
        {
            RootNode newNode = new(position, parent, RootType.Harvester);
            parent.Childrens.Add(newNode);
            nodes.Add(newNode);
            return newNode;
        }
        public bool isNodeLast(RootNode node)
        {
            if(node.Childrens.Count == 0)
                return true;
            else 
                return false;
        }
        public void AddEdge(RootNode nodeA, RootNode nodeB)
        {
            Edge newEdge = new Edge(nodeA, nodeB);
            //nodeA.edges.Add(newEdge);
            //nodeB.edges.Add(newEdge);
        }
        public RootNode FindClosestNode(Vector3 position, float radius)
        {
            RootNode closestNode = null;
            float closestDistance = radius;

            foreach (RootNode node in nodes)
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
