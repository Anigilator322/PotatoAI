using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.RootS
{
    public class RootBluePrint : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private bool isDragging = false;
        private bool isClickedOnRoot
        {
            get
            {
                //if(GridPartition.TryGetNode(radius))
                //return true
                return true;
            }
        }

        private void Start()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerMap.Enable();
            _playerInputActions.PlayerMap.LBMPressed.performed += _ => { if (isClickedOnRoot) TryBlueprint(); };
            _playerInputActions.PlayerMap.LBMPressed.canceled += _ => { isDragging = false; };
        }

        public void TryBlueprint()
        {
            if(isDragging)
                Debug.Log("Trying blueprint");
        }
    }
}
