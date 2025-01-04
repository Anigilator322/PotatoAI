using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using UnityEngine;
using Zenject;
namespace Assets.Scripts.Bootstrap
{
    public class GameBootstrapper : IInitializable
    {
        [Inject] private Plant.Factory _plantFactory;
        [Inject] private RootSpawnSystem spawnSystem;
        public void Initialize()
        {
            var plant = _plantFactory.Create(PlayerRootBuilderInput.PLAYER_ID);
            spawnSystem.SpawnRootNode(plant.Roots, new RootNode(new Vector2(0, 0), null, RootType.Harvester));
        }
    }
}