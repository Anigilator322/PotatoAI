using Assets.Scripts.Bootstrap;
using System;
using UnityEngine;

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

        public void AddResourceClusterAtPoint(Vector2 clusterPosition)
        {
            Bounds soilBounds = _soil.Sprite.bounds;
            float clusterRadius = 1;

            ResourceType resourceType;
            int numberOfDifferentTypes = Enum.GetValues(typeof(ResourceType)).Length;

            for (int i = 0; i < numberOfResourcePoints / 4; i++)
            {
                resourceType = (ResourceType)(i % numberOfDifferentTypes);

                float angle = UnityEngine.Random.Range(0, 360);
                float distance = UnityEngine.Random.Range(0, clusterRadius);

                Vector2 worldPosition = clusterPosition + Rotate(new Vector2(distance, 0), angle);

                SpawnResourcePoint(resourceType, maximumResourcesInPoint, worldPosition);
            }

            Vector2 Rotate(Vector2 v, float degrees)
            {
                float radians = degrees * Mathf.Deg2Rad;
                float cos = Mathf.Cos(radians);
                float sin = Mathf.Sin(radians);
                return new Vector2(
                    v.x * cos,
                    v.x * sin
                );
            }

        }
    }
}
