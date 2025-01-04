using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Metabolics;
using Assets.Scripts.RootS.Plants;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class GameSystemsInstaller : Installer<GameSystemsInstaller>
    {
        public override void InstallBindings()
        {
            //Systems
            Container.Bind<RootBlueprintingSystem>().FromNew().AsSingle();
            Container.Bind<RootSpawnSystem>().FromNew().AsSingle();
            Container.Bind<RootGrowthSystem>().FromNew().AsSingle();
            Container.Bind<MetabolicSystem>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<RootDrawSystem>().FromNew().AsSingle();
        }
    }
}