using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode : PositionedObject
    {
        public List<RootNode> Childrens = new List<RootNode>();
        public RootNode Parent = null;
        public RootNode(Vector3 position)
        {
            Position = position;
        }
        public RootNode(Vector3 position, RootNode parent)
        {
            Position = position;
            Parent = parent;
        }
    }
}


