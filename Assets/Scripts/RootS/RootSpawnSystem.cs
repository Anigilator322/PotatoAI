using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootSpawnSystem
    {
        private PlantRoots _plantRoots;
        private GridPartition<RootNode> _gridPartition;

        public RootSpawnSystem(PlantRoots plantRoots, GridPartition<RootNode> gridPartition)
        {
            _plantRoots = plantRoots;
            _gridPartition = gridPartition;
        }

        public bool TrySpawnRoot(RootNode parent,Vector2 position, RootType rootType)
        {
            RootNode newRoot = new RootNode(position, parent, rootType);
            _plantRoots.Nodes.Add(newRoot);
            _gridPartition.Insert(newRoot);
            return true;
        }
    }
}
