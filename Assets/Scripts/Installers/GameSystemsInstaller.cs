using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class GameSystemsInstaller : Installer<GameSystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GridPartition<RootNode>>().FromNew().AsSingle().WithArguments(1);
        }
    }
}