using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using UnityEngine;
using Zenject;
namespace Assets.Scripts.Bootstrap
{
    public class GameBootstrapper : IInitializable
    {
        private Plant.Factory plantFactory;
        private ResourcePointSpawnSystem resourceSpawnSystem;
        //private MonoBehHelper monoBehHelper;

        public GameBootstrapper(Plant.Factory plantFactory, ResourcePointSpawnSystem resourceSpawnSystem)
        {
            this.plantFactory = plantFactory;
            this.resourceSpawnSystem = resourceSpawnSystem;
        }

        public void Initialize()
        {
            //monoBehHelper = GameObject.FindFirstObjectByType<MonoBehHelper>();

            var plant = plantFactory.Create(PlayerDataModel.PLAYER_ID, Vector2.zero);
            resourceSpawnSystem.FillSoilUniformly();
        }
    }
}