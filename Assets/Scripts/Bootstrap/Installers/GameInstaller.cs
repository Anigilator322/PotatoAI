using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.View;
using Assets.Scripts.UX;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public const string RESOURCES_COLOR = "RESOURCES_COLOR";

        [SerializeField]
        GeneralPrefabs generalPrefabs;

        [SerializeField]
        ResourcePointsConfig resourcePointsConfig;

        public override void InstallBindings()
        {
            // ======= Models =======
            Container.Bind<Soil>().FromComponentInNewPrefab(generalPrefabs.soilPrefab).AsSingle();
            Container.Bind<PlantsModel>().AsSingle();
            Container.Bind<RootNodeContactsModel>().AsSingle();

            Container.Bind<PlantRoots.Factory>().AsSingle();
            Container.Bind<Plant.Factory>().AsCached()
                .WithArguments(generalPrefabs.plantPrefab);


            // ======= Configs =======
            Container.BindInstance(new Dictionary<ResourceType, Color>()
            {
                [ResourceType.Water] = resourcePointsConfig.water,
                [ResourceType.Nitrogen] = resourcePointsConfig.nitrogen,
                [ResourceType.Phosphorus] = resourcePointsConfig.phosphorus,
                [ResourceType.Potassium] = resourcePointsConfig.potassium
            }).WithId(RESOURCES_COLOR);

            Container.BindInstance(resourcePointsConfig);


            // ======= Systems =======
            Container.Bind<RootNodeContactsSystem>().AsSingle()
                .WithArguments(resourcePointsConfig.size);
            Container.BindInterfacesAndSelfTo<ResourceDrawSystem>().AsSingle();
            Container.Bind<ResourcePointSpawnSystem>().AsSingle();
            Container.Bind<RootsBlockSystem>().AsSingle();
            Container.Bind<RootBlueprintingSystem>().AsSingle();
            Container.Bind<RootSpawnSystem>().AsSingle();
            Container.Bind<RootGrowthSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<MetabolicSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<RootDrawSystem>().AsSingle();
            Container.Bind<VisibilitySystem>().FromNew().AsSingle();


            // ======= Bootstrap =======
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle();
            Container.BindInitializableExecutionOrder(typeof(GameBootstrapper), 1);
        }
    }
}