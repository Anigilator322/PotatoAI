using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using UnityEngine;
using Zenject;
namespace Assets.Scripts.Bootstrap
{
    public class GameBootstrapper : IInitializable
    {
        [Inject(Id = "Soil")] private SpriteRenderer soil;
        [Inject] private Plant.Factory plantFactory;
        [Inject] private RootSpawnSystem rootSpawnSystem;
        [Inject] private ResourceSpawnSystem resourceSpawnSystem;
        MonoBehHelper monoBehHelper;

        public void Initialize()
        {
            monoBehHelper = GameObject.FindFirstObjectByType<MonoBehHelper>();

            GameObject.Instantiate(soil);
            var plant = plantFactory.Create(PlayerRootBuilderInput.PLAYER_ID);
            rootSpawnSystem.SpawnRootNodeToPlant(plant.Roots, new RootNode(new Vector2(0, 0), null, RootType.Harvester));

        }

    }
}