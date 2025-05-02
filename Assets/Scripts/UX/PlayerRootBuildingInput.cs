using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Extensions;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.View;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.UX
{
    public class PlayerRootBuildingInput : IInitializable, ITickable
    {
        // This class is a total mess of player UX logic. It should be completely refactored.
        // But of course there is no more perpetual soution than a temporary solution)))
        // I just know it looks clumsy, but i want to spend time on more important tasks, you know

        private InputAction _mousePositionAction;

        [Inject] private PlayerInputActions _playerInputActions;

        [Inject] private PlantsModel _plantsModel;
        [Inject] private PlayerDataModel _playerData;

        [Inject] private RootBlueprintingSystem _rootBlueprintingSystem;
        [Inject] private RootGrowthSystem _rootGrowthSystem;
        [Inject] private MetabolicSystem _metabolicSystem;
        [Inject] private RootDrawSystem _rootDrawSystem;

        TextMeshProUGUI _costIndicator, _justText;

        private DrawingRootBlueprint drawingBlueprint
        {
            get => _playerData.DrawingRootBlueprint;
            set 
            {
                _playerData.DrawingRootBlueprint = value;
                if (_playerData.DrawingRootBlueprint is null)
                    _rootDrawSystem.BlueprintsToDraw = new List<RootBlueprint> { };
                else
                    _rootDrawSystem.BlueprintsToDraw = new List<RootBlueprint> { _playerData.DrawingRootBlueprint.blueprint };
            }
        }

        public PlayerRootBuildingInput(TextMeshProUGUI text, TextMeshProUGUI justText)
        {
            _costIndicator = text;
            _justText = justText;
        }

        private Vector2 GetMousePosition() => Camera.main.ScreenToWorldPoint(_mousePositionAction.ReadValue<Vector2>());

        void IInitializable.Initialize()
        {
            _mousePositionAction = _playerInputActions.PlayerMap.MousePosition;
            InputAction LBMPressedAction = _playerInputActions.PlayerMap.LBMPressed;

            _mousePositionAction.Enable();

            LBMPressedAction.performed += _ =>
            {
                Vector2 mousePosition = GetMousePosition();
                if (IsClickedOnRoot(mousePosition))
                {
                    _playerData.IsBuilding = true;
                    PrepareBlueprint(mousePosition);
                }
            };

            LBMPressedAction.canceled += _ => { _playerData.IsBuilding = false; CancelBlueprinting(); };
        }

        void ITickable.Tick()
        {
            if (!_playerData.IsBuilding)
                return;
            else
                _costIndicator.text = string.Empty;

            Vector2 mousePos = GetMousePosition();

            DrawTrajectory(mousePos);
            UpdateCostIndication(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            var clickedNode = _playerData.playersPlant.Roots.GetNearestAllowedBasementNode(
                _playerData.ClickedNodeSearchRadius,
                mousePosition,
                _playerData.SelectedRootType);

            drawingBlueprint = DrawingRootBlueprint.Create(_playerData.SelectedRootType, clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            if(drawingBlueprint is not null)
                drawingBlueprint = _rootBlueprintingSystem.Update(drawingBlueprint, mousePos);
        }

        private void CancelBlueprinting()
        {
            _costIndicator.text = "";
            
            if (drawingBlueprint is not null
                && (drawingBlueprint.blueprint.RootPath.Count != 0)
                && _metabolicSystem.IsAbleToBuild(drawingBlueprint, _playerData.playersPlant)
                && !drawingBlueprint.IsBlocked)
            {
                _rootGrowthSystem.StartGrowth(drawingBlueprint.blueprint);
                _playerData.playersPlant.Resources.Calories -= _metabolicSystem.CalculateBlueprintPrice(drawingBlueprint);
            }
                
            drawingBlueprint = null;
        }

        private bool IsClickedOnRoot(Vector2 mousePosition)
        {
            return _playerData.playersPlant.Roots
                .GetNodesFromCircle(_playerData.ClickedNodeSearchRadius, mousePosition)
                .Count 
                != 0;
        }

        #region -cost indicator-

        private void UpdateCostIndication(Vector2 mousePosition)
        {
            if (drawingBlueprint is null)
                return;

            var cost = _metabolicSystem.CalculateBlueprintPrice(drawingBlueprint);
            _costIndicator.text = "- " + cost.ToString();

            UpdateCostIndicatorPosition();
        }

        private void UpdateCostIndicatorPosition()
        {
            RectTransform panelRectTransform = _costIndicator.rectTransform;
            Canvas canvas = _costIndicator.canvas;
            Vector2 offset = new Vector2(50, 35);

            if (panelRectTransform == null || canvas == null)
            {
                Debug.LogWarning("Panel RectTransform or Canvas is not assigned.");
                return;
            }

            // Get the mouse position in screen space
            Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();

            // Calculate the desired position with offset
            Vector2 desiredPosition = mousePosition + offset;

            // Convert the desired position to canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                desiredPosition,
                canvas.worldCamera,
                out Vector2 localPoint
            );

            // Adjust the panel's position
            panelRectTransform.localPosition = localPoint;

            // Ensure the panel stays within the camera's viewport
            ClampToViewport(panelRectTransform, canvas);
        }

        #endregion -cost indicator-

        //TODO: move to a separate helper class
        /// <summary>
        /// Clamps the UI element's position to stay within the camera's viewport.
        /// </summary>
        /// <param name="panelRectTransform">The RectTransform of the UI element.</param>
        /// <param name="canvas">The Canvas containing the UI element.</param>
        public static void ClampToViewport(RectTransform panelRectTransform, Canvas canvas)
        {
            // Get the canvas size
            Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

            // Get the panel size
            Vector2 panelSize = panelRectTransform.sizeDelta;

            // Get the current position of the panel
            Vector2 panelPosition = panelRectTransform.anchoredPosition;

            // Calculate the minimum and maximum positions
            float minX = -canvasSize.x / 2 + panelSize.x / 2;
            float maxX = canvasSize.x / 2 - panelSize.x / 2;
            float minY = -canvasSize.y / 2 + panelSize.y / 2;
            float maxY = canvasSize.y / 2 - panelSize.y / 2;

            // Clamp the position
            panelPosition.x = Mathf.Clamp(panelPosition.x, minX, maxX);
            panelPosition.y = Mathf.Clamp(panelPosition.y, minY, maxY);

            // Apply the clamped position
            panelRectTransform.anchoredPosition = panelPosition;
        }
    }
}

