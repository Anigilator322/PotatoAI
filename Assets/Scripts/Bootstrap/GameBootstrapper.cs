using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using UnityEngine;
using Zenject;
namespace Assets.Scripts.Bootstrap
{
    public class GameBootstrapper : IInitializable
    {
        [Inject] private Plant.Factory plantFactory;
        [Inject] private RootSpawnSystem rootSpawnSystem;
        [Inject] private ResourcePointSpawnSystem resourceSpawnSystem;
        MonoBehHelper monoBehHelper;

        Texture2D texture2D;

        public void Initialize()
        {
            monoBehHelper = GameObject.FindFirstObjectByType<MonoBehHelper>();

            var plant = plantFactory.Create(PlayerRootBuilderInput.PLAYER_ID);
            rootSpawnSystem.SpawnRootNodeToPlant(plant.Roots, new RootNode(new Vector2(0, 0), null, RootType.Harvester));
            resourceSpawnSystem.FillSoilUniformly();
        }
    }
}