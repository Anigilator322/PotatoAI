using Assets.Scripts.Map;
using Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Assets.Scripts.Roots
{
    public class RootNode : IPositionedObject, IBlockerNode
    {
        public Transform Transform { get; }
        public RootType Type { get; }

        public bool IsRootBase => Parent is null;
        public RootNode Parent { get; }
        public List<RootNode> Childs { get; } = new List<RootNode>();

        IBlockerNode IBlockerNode.Parent => Parent;

        IReadOnlyList<IBlockerNode> IBlockerNode.Childs => Childs.Select(x => x).ToList<IBlockerNode>();

        public RootNode(Vector2 position, RootNode parent, RootType type)
        {
            Transform = new GameObject().transform;
            Transform.position = position;
            Type = type;
            Transform.name = nameof(this.Type);

            if(parent is not null)
            {
                parent.Childs.Add(this);
                
                Parent = parent;
                Transform.parent = Parent.Transform;
            }
        }

        public RootNode(Vector2 position, RootNode parent)
            : this(position, parent, type: default) {}

        public RootNode(Vector2 position)
            : this(position, null, type: default) { }
    }
}