using Assets.Scripts.RootS.Plants;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

namespace Assets.Scripts.RootS
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

            if(RootNode.Parent != null) 
                RootPath.Add(rootNode.Parent.Position);
            RootPath.Add(rootNode.Position
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
