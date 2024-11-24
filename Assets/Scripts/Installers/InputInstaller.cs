using UnityEngine;
using Zenject;

public class InputInstaller : Installer<InputInstaller>
{
    public override void InstallBindings()
    {
        PlayerInputActions actions = Container.Instantiate<PlayerInputActions>();
        Container.Bind<PlayerInputActions>().FromInstance(actions).AsSingle().NonLazy();
        actions.Enable();
    }
}