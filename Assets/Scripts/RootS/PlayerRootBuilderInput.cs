using Assets.Scripts.Map;
using Assets.Scripts.RootS.Metabolics;
using Assets.Scripts.RootS.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.RootS
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
        private ScaffoldedRootBlueprint _currentBlueprint;
        private InputAction _mousePositionAction;

        private ScaffoldedRootBlueprint blueprintScaffold
        {
            get => _currentBlueprint;
            set 
            {
                _currentBlueprint = value;
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

            blueprintScaffold = _rootBlueprintingSystem.Create(_selectedType, _clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            blueprintScaffold = _rootBlueprintingSystem.Update(blueprintScaffold, mousePos);
        }

        private void CancelBlueprinting()
        {
            if (blueprintScaffold == null || blueprintScaffold.blueprint.RootPath.Count == 0)
                return;
            if (_metabolicSystem.IsAbleToBuild(blueprintScaffold.blueprint))
                _rootGrowthSystem.StartGrowth(blueprintScaffold.blueprint);
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
