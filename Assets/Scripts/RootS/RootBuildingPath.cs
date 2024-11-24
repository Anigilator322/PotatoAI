using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBuildingPath
    {
        public List<Vector2> RootPath { get; private set; } = new List<Vector2>();
        public bool IsNewProcess;
        public bool IsPathCorrect;

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
