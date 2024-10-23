using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using UnityEngine;
using Zenject;

public class GridInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        GridPartition<RootNode> grid = new GridPartition<RootNode>(1);
        Container.Bind<GridPartition<RootNode>>().FromInstance(grid).AsSingle().NonLazy();
    }
}