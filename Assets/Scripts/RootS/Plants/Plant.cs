using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.Plants
{
    public class Plant : MonoBehaviour, IIdentifiable
    {
        public string Id { get; set; }

        public PlantRoots Roots { get; private set; }

        public class Factory : IFactory<Plant>
        {
            private readonly PlantRoots.Factory _rootsFactory;
            private readonly Plant _plantPrefab;
            private readonly PlantsModel _plantsModel;

            public Factory(
                Plant plantPrefab, 
                PlantRoots.Factory rootsFactory,
                PlantsModel plantsModel)
            {
                _plantPrefab = plantPrefab;
                _rootsFactory = rootsFactory;
                _plantsModel = plantsModel;
            }

            public Plant Create(string id)
            {
                Plant plant = GameObject.Instantiate(_plantPrefab);
                plant.Id = id;

                PlantRoots plantRoots = _rootsFactory.Create(plant);
                plant.Roots = plantRoots;
                _plantsModel.Plants.Add(plant);
                return plant;
            }

            public Plant Create()
            {
                return Create(id: null);
            }
        }
    }
}
