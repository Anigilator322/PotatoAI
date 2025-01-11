using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.View;
using System.Diagnostics.Contracts;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class GameSystemsInstaller : Installer<GameSystemsInstaller>
    {
        public override void InstallBindings()
        {
            //Systems
            Container.Bind<RootBlueprintingSystem>().AsSingle();
            Container.Bind<RootSpawnSystem>().AsSingle();
            Container.Bind<RootGrowthSystem>().AsSingle();
            Container.Bind<MetabolicSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<RootDrawSystem>().AsSingle();
            Container.Bind<VisibilitySystem>().FromNew().AsSingle();
        }
    }
}