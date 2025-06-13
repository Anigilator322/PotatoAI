using Assets.Scripts.UX;
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
            Container.BindInterfacesAndSelfTo<PlayerRootBuildingInput>().AsSingle().WithArguments(buildCostIndicator, justText, rootTypeSelection);
            Container.BindInterfacesAndSelfTo<ResourcesIndicationSystem>().AsSingle().WithArguments(resourcesIndicators, caloriesIndicator);
            Container.BindInitializableExecutionOrder(typeof(ResourcesIndicationSystem), 2);

            Container.BindInterfacesAndSelfTo<CameraMoveInput>().FromNew().AsSingle();

            Container.Bind<PlayerDataModel>().AsSingle();
    
            PlayerInputActions actions = new();
            Container.Bind<PlayerInputActions>().FromInstance(actions).NonLazy();
            actions.Enable();
        }
    }
}