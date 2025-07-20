using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots.Plants;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Roots
{
    //This system probably should just iterate over all root nodes prepared to spawn
    public class RootSpawnSystem
    {
        private readonly RootNodeContactsSystem _rootNodeContactsSystem;
        private readonly PlantsModel _plantModel;
        private readonly VisibilitySystem _visibilitySystem;

        public RootSpawnSystem(RootNodeContactsSystem rootNodeContactsSystem, VisibilitySystem visibilitySystem,
            PlantsModel plantModel)
        {
            _plantModel = plantModel;
            _rootNodeContactsSystem = rootNodeContactsSystem;
            _visibilitySystem = visibilitySystem;
        }

        private void Spawn(PlantRoots plantRoots, RootNode newRootNode)
        {
            //Debug.Log("Trying to spawn root node at position: " + newRootNode.Transform);

            plantRoots.AddNode(newRootNode);
            _rootNodeContactsSystem.UpdateContactsByNode(newRootNode);
            _visibilitySystem.UpdateVisibilityForRootNode(plantRoots.plant, newRootNode);
        }

        public RootNode SpawnRootNodeToPlant(PlantRoots plantRoots, RootNode newRootNode)
        {
            Spawn(plantRoots, newRootNode);
            return newRootNode;
        }

        public RootNode SpawnRootNode(RootNode newRootNode)
        {
            PlantRoots plantRoots = _plantModel.Plants.Single(p => p.Roots.Nodes.Contains(newRootNode.Parent)).Roots;
            SpawnRootNodeToPlant(plantRoots, newRootNode);
            return newRootNode;
        }
    }
}
