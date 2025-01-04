using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class InputInstaller : Installer<InputInstaller>
    {
        public override void InstallBindings()
        {
            PlayerInputActions actions = new();
            Container.Bind<PlayerInputActions>().FromInstance(actions).NonLazy();
            actions.Enable();
        }
    }
}