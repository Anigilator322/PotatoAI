using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode
    {
        public Vector3 Position;
        public List<RootNode> Childrens = new List<RootNode>();
        public RootNode Parent = null;
        public RootNode(Vector3 position)
        {
            this.Position = position;
        }
        public RootNode(Vector3 position, RootNode parent)
        {
            this.Position = position;
            this.Parent = parent;
        }
    }
}


