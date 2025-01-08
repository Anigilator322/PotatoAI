using Assets.Scripts.Roots.Plants;
using System;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Assets.Scripts.Roots
{
    public class ResourcePointSpawnSystem
    {
        private readonly int numberOfResourcePoints = 20;
        private readonly float amountInPoint = 10;

        private readonly RootNodeContactsSystem _rootNodeContactsSystem;
        private readonly SoilModel _soil;

        public ResourcePointSpawnSystem(RootNodeContactsSystem rootNodeContactsSystem,
            SoilModel soilResourcesModel,
            int numberOfResourcePoints,
            float amountInPoint)
        {
            _soil = soilResourcesModel;
            _rootNodeContactsSystem = rootNodeContactsSystem;
            this.numberOfResourcePoints = numberOfResourcePoints;
            this.amountInPoint = amountInPoint;
        }

        public ResourcePoint SpawnResourcePoint(ResourceType type, float amount, Vector2 position)
        {
            ResourcePoint newResourcePoint = new ResourcePoint(type, amount, position);
            
            _soil.AddResource(newResourcePoint);   
            _rootNodeContactsSystem.UpdateContactsByResourcePoint(newResourcePoint);
            
            return newResourcePoint;
        }

        public void FillSoilUniformly()
        {
            Bounds soilBounds = _soil.Sprite.bounds;

            ResourceType resourceType;
            int numberOfDifferentTypes = Enum.GetValues(typeof(ResourceType)).Length;

            for (int i = 0; i < numberOfResourcePoints; i++)
            {
                resourceType = (ResourceType)(i % numberOfDifferentTypes);

                float x = UnityEngine.Random.Range(soilBounds.min.x, soilBounds.max.x);
                float y = UnityEngine.Random.Range(soilBounds.min.y, soilBounds.max.y);

                SpawnResourcePoint(resourceType, amountInPoint, new Vector2(x, y));
            }
        }
    }
}
