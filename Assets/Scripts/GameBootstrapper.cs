using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

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
