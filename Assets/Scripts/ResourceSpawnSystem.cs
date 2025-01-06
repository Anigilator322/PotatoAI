using Assets.Scripts.Roots.Plants;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Assets.Scripts.Roots
{
    public class ResourceSpawnSystem
    {
        private readonly RootNodeContactsSystem _rootNodeContactsSystem;
        private readonly SoilResourcesModel _soilResourcesModel;

        public ResourceSpawnSystem(RootNodeContactsSystem rootNodeContactsSystem)
        {
            _rootNodeContactsSystem = rootNodeContactsSystem;
        }

        public ResourcePoint SpawnResourcePoint(ResourceType type, int amount, Vector2 position)
        {
            ResourcePoint newResourcePoint = new ResourcePoint(type, amount, position);
            
            _soilResourcesModel.AddResource(newResourcePoint);   
            _rootNodeContactsSystem.UpdateContactsByResourcePoint(newResourcePoint);
            
            return newResourcePoint;
        }
    }
}
