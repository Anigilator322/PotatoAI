using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class RootBlueprint : IRootBlueprint, IIdentifiable
    {
        private List<Vector2> _rootPath { get; } = new List<Vector2>();


        public string Id { get; }

        public RootType RootType { get; set; }

        public RootNode StartRootNode { get; set; }

        public IReadOnlyList<Vector2> RootPath => _rootPath.AsReadOnly();

        public RootBlueprint(RootType rootType, RootNode startRootNode)
        {
            Id = System.Guid.NewGuid().ToString();
            StartRootNode = startRootNode;
            RootType = rootType;
        }

        public void AppendPoint(Vector2 pathPoint)
        {
            _rootPath.Add(pathPoint);
        }

        public bool TryRemoveLastPoint()
        {
            if(_rootPath.Count == 0) 
                return false;

            _rootPath.RemoveAt(_rootPath.Count - 1);
            return true;
        }

        public void RemoveFirstPoint()
        {
            _rootPath.RemoveAt(0);
        }
    }
}
