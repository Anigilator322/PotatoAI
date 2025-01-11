using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class ScaffoldedRootBlueprint
    {
        public RootBlueprint blueprint { get; }

        List<Vector2> scaffoldedPath = new List<Vector2>();

        public ScaffoldedRootBlueprint(RootType rootType, RootNode startRootNode)
        {
            blueprint = new RootBlueprint(rootType, startRootNode);
            if (blueprint.StartRootNode.Childs.Count == 0
                && startRootNode.Parent is not null)
                scaffoldedPath.Add(startRootNode.Parent.Transform.position);

            scaffoldedPath.Add(startRootNode.Transform.position);
        }

        public IReadOnlyList<Vector2> ScaffoldedPath => scaffoldedPath;

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
            blueprint.AppendPoint(pathPoint);
            scaffoldedPath.Add(pathPoint);
        }

        public void RemoveLastPoint()
        {
            if (blueprint.RootPath.Count > 0)
                blueprint.RemoveLastPoint();
            
            scaffoldedPath.RemoveAt(scaffoldedPath.Count - 1);
        }
    }
}
