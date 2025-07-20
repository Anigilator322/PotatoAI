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
        private Bounds _soilBounds;

        public CameraMoveInput(Soil soil)
        {
            _soilBounds = soil.Sprite.bounds;
        }

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
            var desiredPosition = Camera.main.transform.position + (Vector3)(CurrentSpeed * Speed * Time.deltaTime);
            MoveCameraInBounds(_soilBounds, desiredPosition);
        }

        private void MoveCameraInBounds(Bounds bounds, Vector3 desiredCameraPosition)
        {
            float camHeight = Camera.main.orthographicSize * 2f;
            float camWidth = camHeight * Camera.main.aspect;

            float halfCamHeight = camHeight / 2f;
            float halfCamWidth = camWidth / 2f;
            Vector3 targetPos = desiredCameraPosition;

            float paddingX = 0f;
            float paddingY = 2f;

            float minX = bounds.min.x + halfCamWidth - paddingX;
            float maxX = bounds.max.x - halfCamWidth + paddingX;
            float minY = bounds.min.y + halfCamHeight - paddingY;
            float maxY = bounds.max.y - halfCamHeight + paddingY;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);


            Camera.main.transform.position = new Vector3(targetPos.x, targetPos.y, Camera.main.transform.position.z);
        }
    }
}
