using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using UnityEngine;
using Zenject;

public class GridInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        PlantRoots rootSystem = new PlantRoots();
        GridPartition<PlantRoots> grid = new GridPartition<PlantRoots>(1,rootSystem);
        Container.Bind<GridPartition<PlantRoots>>().FromInstance(grid).AsSingle().NonLazy();
    }
}