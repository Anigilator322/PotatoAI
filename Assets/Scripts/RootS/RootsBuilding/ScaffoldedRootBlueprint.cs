using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class ScaffoldedRootBlueprint : IRootBlueprint
    {
        public RootBlueprint blueprint { get; }

        List<Vector2> _scaffoldedPath = new List<Vector2>();

        public ScaffoldedRootBlueprint(RootType rootType, RootNode startRootNode)
        {
            blueprint = new RootBlueprint(rootType, startRootNode);
            if (blueprint.StartRootNode.Childs.Count == 0
                && startRootNode.Parent is not null)
            {
                _scaffoldedPath.Add(startRootNode.Parent.Transform.position);
            }
            else
            {
                Debug.Log("NEW ROOT");
            }

            _scaffoldedPath.Add(startRootNode.Transform.position);
        }

        public IReadOnlyList<Vector2> RootPath => _scaffoldedPath;

        public RootType RootType { get => blueprint.RootType; set => blueprint.RootType = value; }

        public RootNode StartRootNode { get => blueprint.StartRootNode; set => blueprint.StartRootNode = value; }

        public string Id => blueprint.Id;

        public void AppendPoint(Vector2 pathPoint)
        {
            blueprint.AppendPoint(pathPoint);
            _scaffoldedPath.Add(pathPoint);
        }

        public bool TryRemoveLastPoint()
        {
            if (blueprint.TryRemoveLastPoint())
            {
                _scaffoldedPath.RemoveAt(_scaffoldedPath.Count - 1);
                return true;
            }

            return false;
        }
    }
}
