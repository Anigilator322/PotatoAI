using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootNode : PositionedObject
    {
        public RootType Type;
        public bool isRootBase;
        public int prevNodeInd;
        public List<int> nextNodesIndexes = new List<int>();

        public RootNode(Vector2 position)
        {
            Position = position;
        }

        public RootNode(Vector2 position, int parentInd)
        {
            Position = position;
            prevNodeInd = parentInd;
        }

        public void SetChildren(int childId)
        {
            nextNodesIndexes.Add(childId);
        }
    }
}


