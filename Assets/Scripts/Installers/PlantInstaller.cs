using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Metabolics;
using Assets.Scripts.RootS.Plants;
using Assets.Scripts.RootS.Plants.Factories;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class PlantInstaller : MonoInstaller
    {
        [SerializeField]
        private Plant _plantPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<PlantRoots>().FromNew().AsSingle().NonLazy();
            Container.Bind<RootBlueprintingSystem>().FromNew().AsSingle();
            Container.Bind<RootSpawnSystem>().FromNew().AsSingle();
            Container.Bind<RootGrowthSystem>().FromNew().AsSingle();
            Container.Bind<MetabolicSystem>().FromNew().AsSingle();

            

            Plant plant = Container.InstantiatePrefabForComponent<Plant>(_plantPrefab);
        }

    }
}