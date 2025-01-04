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
            InputInstaller.Install(Container);

            Container.Bind<PlantsModel>().FromNew().AsSingle();

            Container.Bind<PlantRoots.Factory>().AsSingle();
            Container.Bind<Plant.Factory>().AsSingle()
                .WithArguments<Plant>(prefabs.plantPrefab);

            Container.Bind<Plant>()
                .FromMethod(x => Container.Resolve<Plant.Factory>().Create())
                .AsTransient();

            Container.BindInterfacesAndSelfTo<PlayerRootBuilderInput>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().FromNew().AsSingle();
        }
    }
}