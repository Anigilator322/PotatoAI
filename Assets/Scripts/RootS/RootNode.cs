using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public enum RootType
    {
        Harvester = 0,
        Recon = 1,
        Wall = 2
    }

    public class RootNode
    {
        public Vector2 Position;
        public List<RootNode> Childrens = new List<RootNode>();
        public RootNode Parent = null;
        public RootType type;

        public RootNode(Vector2 position)
        {
            this.Position = position;
        }
        public RootNode(Vector2 position, RootNode parent, RootType type)
        {
            this.Position = position;
            this.Parent = parent;
            this.type = type;
        }
    }
}


