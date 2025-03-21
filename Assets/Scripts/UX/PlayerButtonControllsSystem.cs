using Assets.Scripts.Roots;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UX
{
    internal class PlayerButtonControllsSystem
    {
        private PlayerDataModel _playerDataModel;
        private Image _rootTypeSelectionIndicator;
        private Dictionary<RootType, Button> _rootTypeSelectionButtons = new Dictionary<RootType, Button>();

        public PlayerButtonControllsSystem(HorizontalLayoutGroup rootTypeSelection, PlayerDataModel playerDataModel, Image rootTypeSelectionIndicator)
        {
            _rootTypeSelectionIndicator = rootTypeSelectionIndicator;
            _playerDataModel = playerDataModel;

            _rootTypeSelectionButtons = rootTypeSelection.GetComponentsInChildren<Button>()
                .ToDictionary(
                    button => button.GetComponent<RootTypeComponent>().rootType,
                    button => button);
                        
            foreach (RootType rootType in _rootTypeSelectionButtons.Keys) {
                var button = _rootTypeSelectionButtons[rootType];
                button.onClick.AddListener(() => {
                    _playerDataModel.SelectedRootType = button.GetComponent<RootTypeComponent>().rootType;
                    _rootTypeSelectionIndicator.transform.position = button.transform.position;
                });
            }
        }
    }
}
