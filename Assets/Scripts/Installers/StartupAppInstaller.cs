using Assets.Scripts.RootS.Plants;
using UnityEngine;
using Zenject;



public class StartupAppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InputInstaller.Install(Container);
        PlantInstaller.Install(Container);
    }
}