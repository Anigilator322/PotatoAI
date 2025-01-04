using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{

    public class RootBlueprint : IIdentifiable
    {    
        public string Id { get; set; }
        
        public RootType RootType;

        public RootNode RootNode { get; set; }

        public List<Vector2> RootPath { get; private set; } = new List<Vector2>();

        public RootBlueprint(List<Vector2> RootPath)
        {
            this.RootPath = RootPath;
        }

        public RootBlueprint(RootType rootType, RootNode rootNode)
        {
            RootNode = rootNode;
            RootType = rootType;
            Id = System.Guid.NewGuid().ToString(); // Test purposes
            if (RootNode.Parent != null) 
                RootPath.Add(rootNode.Parent.Position);

            RootPath.Add(rootNode.Position);
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
