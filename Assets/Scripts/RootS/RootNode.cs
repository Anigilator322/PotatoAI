using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode : PositionedObject
    {
        public RootType Type;
        public bool isRootBase;
        public RootNode prevNode;
        public List<RootNode> nextNodes = new List<RootNode>();

        public RootNode(Vector2 position)
        {
            Position = position;
        }
        public RootNode(Vector2 position, RootNode parent, RootType type)
        {
            this.Position = position;
            this.Parent = parent;
            this.type = type;

        public RootNode(Vector2 position, RootNode parent)
        {
            Position = position;
            prevNode = parent;
        }

        public void SetChildren(RootNode child)
        {
            nextNodes.Add(child);
        }
    }
}


