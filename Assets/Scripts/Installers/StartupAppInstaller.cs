using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;
using Assets.Scripts.RootS.Plants.Factories;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class StartupAppInstaller : MonoInstaller
    {
        [SerializeField]
        Prefabs prefabs;

        public override void InstallBindings()
        {
            GameSystemsInstaller.Install(Container);

            //Container.Bind<Plant>().WithId(DITools.prefab).FromComponentInNewPrefab(prefabs.plantPrefab).AsSingle();

            Container.Bind<PlantsModel>().FromNew().AsSingle();

            Container.Bind<PlantRoots.Factory>().AsSingle();
            Container.Bind<Plant.Factory>().AsSingle()
                .WithArguments<Plant>(prefabs.plantPrefab);

            Container.Bind<Plant>()
                .FromMethod(x => Container.Resolve<Plant.Factory>().Create())
                .AsTransient();

            InputInstaller.Install(Container);

            //var factory = Container.Resolve<Plant.Factory>();
            Plant plant = Container.Resolve<Plant>();

            Container.Bind<PlayerRootBuilderInput>().FromNewComponentOn(plant.gameObject).AsSingle().NonLazy();
        }
    }
}