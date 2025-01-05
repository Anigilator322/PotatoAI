using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class RootBlueprint : IIdentifiable
    {    
        public string Id { get; }
        
        public RootType RootType { get; }

        public RootNode StartRootNode { get; set; }

        public List<Vector2> RootPath { get; } = new List<Vector2>();

        public RootBlueprint(RootType rootType, RootNode startRootNode)
        {
            Id = System.Guid.NewGuid().ToString();
            StartRootNode = startRootNode;
            RootType = rootType;
        }

        public void AppendPoint(Vector2 pathPoint)
        {
            RootPath.Add(pathPoint);
        }

        public void RemoveLastPoint()
        {
            RootPath.RemoveAt(RootPath.Count - 1);
        }
    }
}
