using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Metabolics;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using Assets.Scripts.Roots.View;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.UX
{
    public class PlayerRootBuilderInput : IInitializable, ITickable
    {
        // This class is a total mess of player UX logic. It should be completely refactored.
        public const string PLAYER_ID = "player_1";

        [SerializeField] private float _clickedNodeSearchRadius = 2f;

        [Inject] private PlayerInputActions _playerInputActions;
        [Inject] private RootBlueprintingSystem _rootBlueprintingSystem;
        [Inject] private RootGrowthSystem _rootGrowthSystem;
        [Inject] private MetabolicSystem _metabolicSystem;
        [Inject] private RootDrawSystem _rootDrawSystem;
        [Inject] private PlantsModel _plantsModel;

        TextMeshProUGUI _costIndicator, _justText;

        public PlayerRootBuilderInput(TextMeshProUGUI text, TextMeshProUGUI justText)
        {
            _costIndicator = text;
            _justText = justText;
        }

        private bool _isBuilding = false;
        private Plant _playersPlant;
        private Plant playersPlant
        { 
            get
            {
                if (_playersPlant is null)
                {
                    _playersPlant = _plantsModel.Plants
                        .Single(p => p.Id == PLAYER_ID);
                }

                return _playersPlant;
            } 
        }
        private RootNode _clickedNode;
        private RootType _selectedType = RootType.Harvester;
        private DrawingRootBlueprint _currentBlueprint;
        private InputAction _mousePositionAction;

        private DrawingRootBlueprint drawingBlueprint
        {
            get => _currentBlueprint;
            set 
            {
                _currentBlueprint = value;
                if (_currentBlueprint is null)
                    _rootDrawSystem.BlueprintsToDraw = new List<RootBlueprint> { };
                else
                    _rootDrawSystem.BlueprintsToDraw = new List<RootBlueprint> { _currentBlueprint.blueprint };
            }
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
                    _isBuilding = true;
                    PrepareBlueprint(mousePosition);
                }
            };

            LBMPressedAction.canceled += _ => { _isBuilding = false; CancelBlueprinting(); };
        }

        void ITickable.Tick()
        {
            Vector2 mousePos = GetMousePosition();

            _justText.text = mousePos.ToString() + "\n"
                + _mousePositionAction.ReadValue<Vector2>().ToString();

            if (!_isBuilding)
                return;
            else
                _costIndicator.text = string.Empty;
            


            DrawTrajectory(mousePos);
            UpdateCostIndication(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            List<RootNode> queiriedNodes = playersPlant.Roots.GetNodesFromCircle(_clickedNodeSearchRadius, mousePosition);
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);

            drawingBlueprint = _rootBlueprintingSystem.Create(_selectedType, _clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            drawingBlueprint = _rootBlueprintingSystem.Update(drawingBlueprint, mousePos);
        }

        private void CancelBlueprinting()
        {
            _costIndicator.text = "";
            
            if (drawingBlueprint is not null
                && (drawingBlueprint.blueprint.RootPath.Count != 0)
                && _metabolicSystem.IsAbleToBuild(drawingBlueprint, playersPlant))
            {
                _rootGrowthSystem.StartGrowth(drawingBlueprint.blueprint);
                _playersPlant.Resources.Calories -= _metabolicSystem.CalculateBlueprintPrice(drawingBlueprint);
            }
                
            drawingBlueprint = null;
        }

        private bool IsClickedOnRoot(Vector2 mousePosition)
        {

            if (playersPlant.Roots.GetNodesFromCircle(_clickedNodeSearchRadius, mousePosition).Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private RootNode FindClosestNodeToMouse(List<RootNode> rootNodes, Vector2 mousePosition)
        {
            RootNode closestNode = rootNodes[0];
            float minDistance = Vector2.Distance(mousePosition, (Vector2)closestNode.Transform.position);
            foreach (RootNode node in rootNodes)
            {
                if (Vector2.Distance((Vector2)node.Transform.position, mousePosition) < minDistance)
                {
                    closestNode = node;
                    minDistance = Vector2.Distance((Vector2)node.Transform.position, mousePosition);
                }
            }
            return closestNode;
        }

        #region -cost indicator-


        private void UpdateCostIndication(Vector2 mousePosition)
        {
            var cost = _metabolicSystem.CalculateBlueprintPrice(drawingBlueprint);
            _costIndicator.text = "- " + cost.ToString();

            FollowMouse(_costIndicator.rectTransform, _costIndicator.canvas, new Vector2(50, 35));
        }

        //TODO: DECOMPOSE
        private void FollowMouse(RectTransform panelRectTransform, Canvas canvas, Vector2 offset)
        {
            if (panelRectTransform == null || canvas == null)
            {
                Debug.LogWarning("Panel RectTransform or Canvas is not assigned.");
                return;
            }

            //TODO: FIX
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

        /// <summary>
        /// Clamps the UI element's position to stay within the camera's viewport.
        /// </summary>
        /// <param name="panelRectTransform">The RectTransform of the UI element.</param>
        /// <param name="canvas">The Canvas containing the UI element.</param>
        private static void ClampToViewport(RectTransform panelRectTransform, Canvas canvas)
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

        #endregion -cost indicator-
    }
}

