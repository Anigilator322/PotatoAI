using Assets.Scripts.RootS.Plants;
using Assets.Scripts.RootS.Plants.Factories;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class StartupAppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InputInstaller.Install(Container);
            GameSystemsInstaller.Install(Container);
        }
    }
}