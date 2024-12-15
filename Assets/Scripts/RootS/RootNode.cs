using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode : PositionedObject
    {
        public RootType Type = RootType.Harvester;

        public bool IsRootBase;
        public RootNode Parent;
        public List<RootNode> Childs = new List<RootNode>();

        public RootNode(Vector2 position)
        {
            IsRootBase = true;
            Position = position;
        }
        
        public RootNode(Vector2 position, RootNode parent, RootType type)
        {
            this.Position = position;
            this.Parent = parent;
            this.Type = type;
        }

        public RootNode(Vector2 position, RootNode parent)
        {
            Position = position;
            this.Parent = parent;
        }

        public RootNode()
        {
        }

        public void SetChildren(RootNode child)
        {
            Childs.Add(child);
        }
    }
}