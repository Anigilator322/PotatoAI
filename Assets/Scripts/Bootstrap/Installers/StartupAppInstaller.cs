using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class StartupAppInstaller : MonoInstaller
    {
        [SerializeField]
        GeneralPrefabs generalPrefabs;

        [SerializeField]
        ResourcePointsConfig resourcePointsConfig;

        [SerializeField]
        VerticalLayoutGroup resourcesIndicator;

        [SerializeField]
        TextMeshProUGUI caloriesIndicator, buildCostIndicator, justText;

        [SerializeField]
        HorizontalLayoutGroup rootTypeSelection;

        [SerializeField]
        Image rootTypeSelectionIndicator;

        public override void InstallBindings()
        {
            // ======= Models =======
            Container.Bind<Soil>().FromComponentInNewPrefab(generalPrefabs.soilPrefab).AsSingle();
            Container.Bind<PlantsModel>().AsSingle();
            Container.Bind<RootNodeContactsModel>().AsSingle();

            Container.Bind<PlantRoots.Factory>().AsSingle();
            Container.Bind<Plant.Factory>().AsSingle()
                .WithArguments(generalPrefabs.plantPrefab);

            Container.Bind<Plant>()
                .FromMethod(x => Container.Resolve<Plant.Factory>().Create())
                .AsTransient();

            Container.Bind<PlayerDataModel>().AsSingle();

            // ======= Systems =======
            Container.Bind<RootNodeContactsSystem>().AsSingle()
                .WithArguments(resourcePointsConfig.size);

            var colorsDict = new Dictionary<ResourceType, Color>()
            {
                [ResourceType.Water] = resourcePointsConfig.water,
                [ResourceType.Nitrogen] = resourcePointsConfig.nitrogen,
                [ResourceType.Phosphorus] = resourcePointsConfig.phosphorus,
                [ResourceType.Potassium] = resourcePointsConfig.potassium
            };

            Container.BindInterfacesAndSelfTo<ResourceDrawSystem>()
                .FromMethod(x =>
                {
                    return new ResourceDrawSystem(
                        Container.Resolve<Soil>(),
                        resourcePointsConfig.size,
                        colorsDict,
                        resourcePointsConfig.maximumResourcesInPoint,
                        Container.Resolve<RootNodeContactsModel>());
                })
                .AsSingle();

            Container.BindInstance(
                new ResourcePointSpawnSystem(
                    Container.Resolve<RootNodeContactsSystem>(),
                    Container.Resolve<Soil>(),
                    resourcePointsConfig.numberOfResourcePoints,
                    resourcePointsConfig.maximumResourcesInPoint))
                .AsSingle();

            GameSystemsInstaller.Install(Container);
            InputInstaller.Install(Container);


            Container.BindInterfacesAndSelfTo<CameraMoveInput>().FromNew().AsSingle();

            Container.Bind<PlayerButtonControllsSystem>().FromNew().AsSingle().WithArguments(rootTypeSelection, rootTypeSelectionIndicator).NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerRootBuildingInput>().AsSingle().WithArguments(buildCostIndicator, justText);

            Container.BindInterfacesAndSelfTo<ResourcesViewSystem>().AsSingle().WithArguments(resourcesIndicator, caloriesIndicator, colorsDict).Lazy();
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().FromNew().AsSingle().NonLazy();

            GameBootstrapper gameBootstrapper = Container.Resolve<GameBootstrapper>();
            gameBootstrapper.Start();
        }
    }
}