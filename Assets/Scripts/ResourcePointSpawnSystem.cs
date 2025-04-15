using Assets.Scripts.Bootstrap;
using Assets.Scripts.Roots.Plants;
using System;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Assets.Scripts.Roots
{
    public class ResourcePointSpawnSystem
    {
        private readonly int numberOfResourcePoints = 20;
        private readonly float maximumResourcesInPoint = 10;

        private readonly RootNodeContactsSystem _rootNodeContactsSystem;
        private readonly Soil _soil;

        public ResourcePointSpawnSystem(RootNodeContactsSystem rootNodeContactsSystem,
            Soil soilResourcesModel,
            ResourcePointsConfig resourcePointsConfig)
        {
            _soil = soilResourcesModel;
            _rootNodeContactsSystem = rootNodeContactsSystem;
            this.numberOfResourcePoints = resourcePointsConfig.numberOfResourcePoints;
            this.maximumResourcesInPoint = resourcePointsConfig.maximumResourcesInPoint;
        }

        public ResourcePoint SpawnResourcePoint(ResourceType type, float amount, Vector2 position)
        {
            ResourcePoint newResourcePoint = new ResourcePoint(type, amount, position);
            
            _soil.AddResource(newResourcePoint);   
            _rootNodeContactsSystem.UpdateContactsByResourcePoint(newResourcePoint);
            
            return newResourcePoint;
        }

        public void DestroyResourcePoint(ResourcePoint resourcePoint)
        {
            _soil.RemoveResource(resourcePoint);
            _rootNodeContactsSystem.UpdateContactsByResourcePoint(resourcePoint);
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

                Vector2 worldPosition = new Vector2(x, y);

                SpawnResourcePoint(resourceType, maximumResourcesInPoint, worldPosition);
            }
        }
    }
}
