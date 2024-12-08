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

        public RootNode TrySpawnRoot(GrowingRoot root)
        {
            RootNode newRoot = new RootNode(root.Blueprint.RootPath[0], root.Blueprint.RootNode, root.Blueprint.RootType);
            _plantRoots.Nodes.Add(newRoot);
            _gridPartition.Insert(newRoot);
            return newRoot;
        }
    }
}
