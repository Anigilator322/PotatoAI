using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class StartupAppInstaller : MonoInstaller
    {
        [SerializeField]
        Prefabs prefabs;

        public override void InstallBindings()
        {
            Container.Bind<RootNodeContactsModel>().AsSingle();
            Container.Bind<SoilResourcesModel>().AsSingle();
            Container.Bind<PlantsModel>().AsSingle();

            Container.Bind<SpriteRenderer>().WithId("Soil").FromInstance(prefabs.soilPrefab).AsSingle();

            GameSystemsInstaller.Install(Container);
            InputInstaller.Install(Container);

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