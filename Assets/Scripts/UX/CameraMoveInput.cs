using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.View;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.UX
{
    public class CameraMoveInput : IInitializable, ITickable
    {
        [SerializeField] public float Speed = 6f;

        [Inject] private PlayerInputActions _playerInputActions;
        
        private InputAction _rightArrowInput;

        Vector2 TargetSpeed, CurrentSpeed;

        void IInitializable.Initialize()
        {
            _rightArrowInput = _playerInputActions.PlayerMap.Arrows;
            _rightArrowInput.performed += context => { 
                Vector2 value = context.ReadValue<Vector2>(); 
                Move(value);
            };

            _rightArrowInput.canceled += context => {
                Move(Vector2.zero);
            };

            _rightArrowInput.Enable();
        }

        void Move(Vector2 TargetSpeed)
        {
            this.TargetSpeed = TargetSpeed;
        }

        void ITickable.Tick()
        {
           CurrentSpeed = Vector2.Lerp(CurrentSpeed, TargetSpeed, 0.12f);       
           Camera.main.transform.position += (Vector3)(CurrentSpeed * Speed * Time.deltaTime);
        }
    }
}
