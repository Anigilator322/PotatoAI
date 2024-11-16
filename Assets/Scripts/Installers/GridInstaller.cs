using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using UnityEngine;
using Zenject;

public class GridInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        RootSystem rootSystem = new RootSystem();
        GridPartition<RootSystem> grid = new GridPartition<RootSystem>(1,rootSystem);
        Container.Bind<GridPartition<RootSystem>>().FromInstance(grid).AsSingle().NonLazy();
    }
}