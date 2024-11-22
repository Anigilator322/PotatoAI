using Assets.Scripts.Map;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.RootS
{
    public class RootBluePrint : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        [Inject] private GridPartition<PlantRoots> _gridPartition;
        private bool isDragging = false;

        private bool isClickedOnRoot(Vector2 mousePos)
        {
            
            //if(_gridPartition.Query(2f,mousePos).Count != 0)
            {
                return true;
            }
            //else
            {
                return true;
            }
             
            
        }

        private void Start()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerMap.Enable();
            _playerInputActions.PlayerMap.LBMPressed.performed += _ => { if (isClickedOnRoot(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>())) isDragging=true; };
            _playerInputActions.PlayerMap.LBMPressed.canceled += _ => { isDragging = false; };
        }

        private void Update()
        {
            TryBlueprint();
        }

        public void TryBlueprint()
        {
            if (!isDragging)
                return;
            Debug.Log("Dragging");

            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();


        }
    }
}
