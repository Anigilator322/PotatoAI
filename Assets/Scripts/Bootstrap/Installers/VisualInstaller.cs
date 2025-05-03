using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VisualInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<VisibilitySystem>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<RootDrawSystem>().AsSingle();
        Container.BindInitializableExecutionOrder(typeof(RootDrawSystem), 2);
        Container.BindInterfacesAndSelfTo<ResourceDrawSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<MeshCache>().AsSingle();
        Container.BindInitializableExecutionOrder(typeof(MeshCache), 1);
    }
}
