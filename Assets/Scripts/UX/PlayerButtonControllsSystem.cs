using Assets.Scripts.Roots;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UX
{
    internal class PlayerButtonControllsSystem
    {
        private PlayerDataModel _playerDataModel;

        public PlayerButtonControllsSystem(HorizontalLayoutGroup rootTypeSelection, PlayerDataModel playerDataModel)
        {
            _playerDataModel = playerDataModel;

            Debug.Log("SHEESH IM HERE");
            foreach (Button button in rootTypeSelection.GetComponentsInChildren<Button>()) {
                button.onClick.AddListener(() => {
                    _playerDataModel.SelectedRootType  = button.GetComponent<RootTypeComponent>().rootType;
                    Debug.Log(_playerDataModel.SelectedRootType);
                });
            }
        }
    }
}
