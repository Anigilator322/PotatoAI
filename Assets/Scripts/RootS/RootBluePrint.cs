using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.RootS
{
    public class RootBluePrint : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;

        private void Start()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerMap.Enable();
            _playerInputActions.PlayerMap.LBMPressed.performed += TryBlueprint;
        }

        public void TryBlueprint(InputAction.CallbackContext context)
        {
            
        }
    }
}
