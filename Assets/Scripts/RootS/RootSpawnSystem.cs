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
            RootNode newRootNode = new RootNode(root.Blueprint.RootPath[0], root.Blueprint.RootNode, root.Blueprint.RootType);
            root.Blueprint.RootNode.Childs.Add(newRootNode);
            Debug.Log("Trying to spawn root at position: " + newRootNode.Position);
            _plantRoots.Nodes.Add(newRootNode);
            _gridPartition.Insert(newRootNode);
            return newRootNode;
        }
    }
}
