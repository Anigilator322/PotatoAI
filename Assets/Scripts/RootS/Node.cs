using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class Node
    {
        public Vector3 Position;
        public List<Node> Childrens = new List<Node>();
        public Node Parent = null;
        public Node(Vector3 position)
        {
            this.Position = position;
        }
        public Node(Vector3 position, Node parent)
        {
            this.Position = position;
            this.Parent = parent;
        }
    }
}


