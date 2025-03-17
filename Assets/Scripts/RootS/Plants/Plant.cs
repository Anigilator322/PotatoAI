using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.Plants
{
    public class Plant : MonoBehaviour, IIdentifiable
    {
        public string Id { get; set; }

        public PlantRoots Roots { get; private set; }

        public PlantResources Resources = new PlantResources();

        public class Factory : IFactory<Plant>
        {
            private readonly PlantRoots.Factory _rootsFactory;
            private readonly Plant _plantPrefab;
            private readonly PlantsModel _plantsModel;
            private readonly Soil _soil;
            private readonly RootSpawnSystem _rootSpawnSystem;

            public Factory(
                Plant plantPrefab, 
                PlantRoots.Factory rootsFactory,
                PlantsModel plantsModel,
                Soil soilModel,
                RootSpawnSystem rootSpawnSystem)
            {
                _plantPrefab = plantPrefab;
                _rootsFactory = rootsFactory;
                _plantsModel = plantsModel;
                _soil = soilModel;
                this._rootSpawnSystem = rootSpawnSystem;
            }

            public Plant Create(string id, Vector2 rootBasePosition)
            {
                Plant plant = Instantiate(_plantPrefab, (Vector3)rootBasePosition, Quaternion.identity, _soil.transform);
                plant.Id = id;

                plant.Resources.Calories = 100;

                plant.Roots = _rootsFactory.Create(plant);

                _rootSpawnSystem.SpawnRootNodeToPlant(plant.Roots, new RootNode(new Vector2(0, 0), null, RootType.Harvester));

                _plantsModel.Plants.Add(plant);
                return plant;
            }

            public Plant Create()
            {
                return Create(id: null, rootBasePosition: Vector2.zero);
            }
        }
    }
}
