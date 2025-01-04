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
    public class PlayerRootBuilderInput : IInitializable, ITickable
    {
        public const string PLAYER_ID = "player_1";

        [SerializeField] private float _clickedNodeSearchRadius = 2f;

        [Inject] private PlayerInputActions _playerInputActions;
        [Inject] private RootBlueprintingSystem _rootBlueprintingSystem;
        [Inject] private RootGrowthSystem _rootGrowthSystem;
        [Inject] private MetabolicSystem _metabolicSystem;
        [Inject] private RootDrawSystem _rootDrawSystem;
        [Inject] private PlantsModel _plantsModel;

        private bool _isDragging = false;
        private PlantRoots _playersPlantRoots;
        private PlantRoots PlayersPlantRoots 
        { 
            get
            {
                if (_playersPlantRoots is null)
                {
                    _playersPlantRoots = _plantsModel.Plants
                        .Single(p => p.Id == PLAYER_ID)
                            .Roots;
                }

                return _playersPlantRoots;
            } 
        }
        private RootNode _clickedNode;
        private RootType _selectedType = RootType.Harvester;
        private RootBlueprint _currentBlueprint;
        private InputAction _mousePositionAction;

        private RootBlueprint currentBlueprint
        {
            get => _currentBlueprint;
            set 
            {
                _currentBlueprint = value;
                _rootDrawSystem.BlueprintsToDraw = new List<RootBlueprint> { _currentBlueprint };
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
                    _isDragging = true;
                    PrepareBlueprint(mousePosition);
                }
            };

            LBMPressedAction.canceled += _ => { _isDragging = false; CancelBlueprinting(); };
        }

        void ITickable.Tick()
        {
            if (!_isDragging)
                return;
            Debug.Log("Drawing trajectory");
            Debug.Log("Mouse position: " + GetMousePosition());
            Vector2 mousePos = GetMousePosition();
            DrawTrajectory(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            List<RootNode> queiriedNodes = PlayersPlantRoots.GetNodesFromCircle(_clickedNodeSearchRadius, mousePosition);
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);

            currentBlueprint = _rootBlueprintingSystem.Create(_selectedType, _clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            currentBlueprint = _rootBlueprintingSystem.Update(currentBlueprint, mousePos);
        }

        private void CancelBlueprinting()
        {
            if (currentBlueprint == null)
                return;
            if (_metabolicSystem.IsAbleToBuild(currentBlueprint))
                _rootGrowthSystem.StartGrowth(currentBlueprint);
        }

        private bool IsClickedOnRoot(Vector2 mousePosition)
        {

            if (PlayersPlantRoots.GetNodesFromCircle(_clickedNodeSearchRadius, mousePosition).Count != 0)
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
            float minDistance = Vector2.Distance(mousePosition, closestNode.Position);
            foreach (RootNode node in rootNodes)
            {
                if (Vector2.Distance(node.Position, mousePosition) < minDistance)
                {
                    closestNode = node;
                    minDistance = Vector2.Distance(node.Position, mousePosition);
                }
            }
            return closestNode;
        }
    }
}
