using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBlueprint
    {
        public RootType RootType;
        public RootNode origin;
        public List<Vector2> RootPath { get; private set; } = new List<Vector2>();
        public bool IsNewProcess;
        public bool IsPathCorrect;

        public RootBlueprint(RootType rootType, RootNode origin)
        {
            RootType = rootType;
            this.origin = origin;

            IsNewProcess = origin.nextNodes.Count != 0;
            if (origin.prevNode != null)
                AddInPath(origin.prevNode.Position);
            AddInPath(origin.Position);
        }

        public void AddInPath(Vector2 pathPoint)
        {
            RootPath.Add(pathPoint);
        }
        public void RemoveFromPathLastPoint()
        {
            RootPath.RemoveAt(RootPath.Count-1);
        }

        

    }
}
