using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class StartupAppInstaller : MonoInstaller
    {
        [SerializeField]
        GeneralPrefabs generalPrefabs;

        [SerializeField]
        ResourcePointsConfig resourcePointsConfig;

        public override void InstallBindings()
        {
            // ======= Models =======
            Container.Bind<RootNodeContactsModel>().AsSingle();
            Container.Bind<PlantsModel>().AsSingle();
            Container.Bind<SoilModel>().FromComponentInNewPrefab(generalPrefabs.soilPrefab).AsSingle();

            Container.Bind<PlantRoots.Factory>().AsSingle();
            Container.Bind<Plant.Factory>().AsSingle()
                .WithArguments(generalPrefabs.plantPrefab);

            Container.Bind<Plant>()
                .FromMethod(x => Container.Resolve<Plant.Factory>().Create())
                .AsTransient();

            // ======= Systems =======
            Container.BindInterfacesAndSelfTo<ResourceDrawSystem>()
                .FromMethod(x =>
                {
                    var colorsDict = new Dictionary<ResourceType, Color>()
                    {
                        [ResourceType.Water] = resourcePointsConfig.water,
                        [ResourceType.Nitrogen] = resourcePointsConfig.nitrogen,
                        [ResourceType.Phosphorus] = resourcePointsConfig.phosphorus,
                        [ResourceType.Potassium] = resourcePointsConfig.potassium
                    };

                    return new ResourceDrawSystem(
                        Container.Resolve<SoilModel>(),
                        resourcePointsConfig.size,
                        colorsDict,
                        resourcePointsConfig.maximumResourcesInPoint);
                })
                .AsSingle();

            Container.Bind<RootNodeContactsSystem>().AsSingle()
                .WithArguments(resourcePointsConfig.size);

            Container.BindInstance(
                new ResourcePointSpawnSystem(
                    Container.Resolve<RootNodeContactsSystem>(),
                    Container.Resolve<SoilModel>(),
                    resourcePointsConfig.numberOfResourcePoints,
                    resourcePointsConfig.maximumResourcesInPoint))
                .AsSingle();

            //Container.Bind<ResourcePointSpawnSystem>()
            //    .FromInstance(new ResourcePointSpawnSystem(
            //        Container.Resolve<RootNodeContactsSystem>(),
            //        Container.Resolve<SoilModel>(),
            //        resourcePointsConfig.numberOfResourcePoints,
            //        resourcePointsConfig.maximumResourcesInPoint))
            //    .AsSingle();

            GameSystemsInstaller.Install(Container);
            InputInstaller.Install(Container);

            Container.BindInterfacesAndSelfTo<PlayerRootBuilderInput>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().FromNew().AsSingle();
        }
    }
}