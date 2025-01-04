using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode : PositionedObject
    {
        public RootType Type { get; }

        public bool IsRootBase => Parent is null;
        public RootNode Parent { get; }
        public List<RootNode> Childs { get; } = new List<RootNode>();

        public RootNode(Vector2 position, RootNode parent, RootType type)
        {
            this.Position = position;
            this.Type = type;

            if(parent is not null)
            {
                this.Parent = parent;
                parent.Childs.Add(this);
            }
        }

        public RootNode(Vector2 position, RootNode parent)
            : this(position, parent, type: default) {}

        public RootNode(Vector2 position)
            : this(position, null, type: default) { }
    }
}