using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    //This system probably should just iterate over all root nodes prepared to spawn
    public class RootSpawnSystem
    {
        //TODO: How do we establish link between newly added RootNode and plant? 
        //Why do we think it belongs to this instance of PlantRoots?

        public RootNode SpawnRootNode(PlantRoots plantRoots, RootNode parent, Vector2 position, RootType type)
        {
            RootNode newRootNode = new RootNode(position, parent, type);

            Spawn(plantRoots, newRootNode);
            return newRootNode;
        }

        public RootNode SpawnRootNode(PlantRoots plantRoots, RootNode newRootNode)
        {
            Spawn(plantRoots, newRootNode);
            return newRootNode;
        }

        private void Spawn(PlantRoots plantRoots, RootNode newRootNode)
        {
            Debug.Log("Trying to spawn root at position: " + newRootNode.Position);
            plantRoots.AddNode(newRootNode);
        }
    }
}
