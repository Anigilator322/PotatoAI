using Assets.Scripts.RootS.Plants;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class ScaffoldedRootBlueprint
    {
        public RootBlueprint rootBlueprint { get; }

        List<Vector2> scaffoldedPath = new List<Vector2>();

        public ScaffoldedRootBlueprint(RootType rootType, RootNode startRootNode)
        {
            rootBlueprint = new RootBlueprint(rootType, startRootNode);
            if (rootBlueprint.StartRootNode.Childs.Count == 0
                && startRootNode.Parent is not null)
                scaffoldedPath.Add(startRootNode.Parent.Position);

            scaffoldedPath.Add(startRootNode.Position);
        }

        public IReadOnlyList<Vector2> RootPath => scaffoldedPath;

        //public int PointsCount()
        //{
        //    return rootBlueprint.RootPath.Count;
        //}

        //public Vector2 GetPointByIndex(int index)
        //{
        //    return rootBlueprint.RootPath[index];
        //}

        public void AppendPoint(Vector2 pathPoint)
        {
            rootBlueprint.AppendPoint(pathPoint);
            scaffoldedPath.Add(pathPoint);
        }

        public void RemoveLastPoint()
        {
            if (rootBlueprint.RootPath.Count > 0)
                rootBlueprint.RemoveLastPoint();
            
            scaffoldedPath.RemoveAt(scaffoldedPath.Count - 1);
        }
    }

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
