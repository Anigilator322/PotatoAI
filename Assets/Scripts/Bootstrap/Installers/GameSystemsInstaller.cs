using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.View;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
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
            Container.Bind<FieldOfView>().FromNew().AsSingle().NonLazy();
        }
    }
}