using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using UnityEngine;
using Zenject;

public class GridInstaller : MonoInstaller
{
    [SerializeField] private Vector2 plantRootOrigin;
    public override void InstallBindings()
    {
        GridPartition<PlantRoots> grid = new GridPartition<PlantRoots>(1);
        Container.Bind<GridPartition<PlantRoots>>().FromInstance(grid).AsSingle().NonLazy();

        PlantRoots rootSystem = new PlantRoots(plantRootOrigin);
        Container.Bind<PlantRoots>().FromInstance(rootSystem);
    }
}