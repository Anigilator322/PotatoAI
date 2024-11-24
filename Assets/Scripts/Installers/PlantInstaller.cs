using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;
using UnityEngine;
using Zenject;

public class PlantInstaller : Installer<PlantInstaller>
{

    private Plant _plantPrefab;
    private Transform _plantSpawnPosition;

    public override void InstallBindings()
    {
        Container.Bind<PlantRoots>().FromNew().AsSingle();
        Container.Bind<GridPartition<RootNode>>().FromNew().AsSingle().WithArguments(1);
        Plant plant = Container.InstantiatePrefabForComponent<Plant>(_plantPrefab,_plantSpawnPosition.position,Quaternion.identity,null);
    }

}