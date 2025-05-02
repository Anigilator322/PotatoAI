using Assets.Scripts.UX;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Bootstrap.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        VerticalLayoutGroup resourcesIndicators;

        [SerializeField]
        TextMeshProUGUI caloriesIndicator, buildCostIndicator, justText;

        [SerializeField]
        HorizontalLayoutGroup rootTypeSelection;

        [SerializeField]
        Image rootTypeSelectionIndicator;

        public override void InstallBindings()
        {
            Container.Bind<PlayerButtonControllsSystem>().AsSingle().WithArguments(rootTypeSelection, rootTypeSelectionIndicator).NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerRootBuildingInput>().AsSingle().WithArguments(buildCostIndicator, justText);
            Container.BindInterfacesAndSelfTo<ResourcesViewSystem>().AsSingle().WithArguments(resourcesIndicators, caloriesIndicator);
            Container.BindInitializableExecutionOrder(typeof(ResourcesViewSystem), 2);

            Container.BindInterfacesAndSelfTo<CameraMoveInput>().FromNew().AsSingle();

            Container.Bind<PlayerDataModel>().AsSingle();

            PlayerInputActions actions = new();
            Container.Bind<PlayerInputActions>().FromInstance(actions).NonLazy();
            actions.Enable();
        }
    }
}