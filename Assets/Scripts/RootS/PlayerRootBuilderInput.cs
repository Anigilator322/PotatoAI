using Assets.Scripts.Map;
using Assets.Scripts.RootS.Metabolics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class PlayerRootBuilderInput : MonoBehaviour
    {
        [SerializeField] private float _clickedNodeSearchRadius = 2f;

        private PlayerInputActions _playerInputActions;
        private GridPartition<RootNode> _gridPartition;
        private RootBlueprintingSystem _rootBlueprintingSystem;
        private RootGrowthSystem _rootGrowthSystem;
        private MetabolicSystem _metabolicSystem;

        private bool _isDragging = false;
        private RootNode _clickedNode;
        private RootType _selectedType = RootType.Harvester;
        private RootBlueprint _currentBlueprint;

        void Start()
        {
            _playerInputActions.PlayerMap.LBMPressed.performed += _ =>
            {
                if (IsClickedOnRoot(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>()))
                {
                    _isDragging = true;
                    PrepareBlueprint(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>());
                }
            };
            _playerInputActions.PlayerMap.LBMPressed.canceled += _ => { _isDragging = false; CancelBlueprinting(); };
        }

        void Update()
        {
            if (!_isDragging)
                return;
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();
            DrawTrajectory(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            List<RootNode> queiriedNodes = _gridPartition.Query(_clickedNodeSearchRadius, mousePosition);
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);

            _currentBlueprint = _rootBlueprintingSystem.Create(_selectedType, _clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            _currentBlueprint = _rootBlueprintingSystem.Update(_currentBlueprint, mousePos);
        }

        private void CancelBlueprinting()
        {
            if (_metabolicSystem.IsAbleToBuild(_currentBlueprint))
                _rootGrowthSystem.StartGrowth(_currentBlueprint);
        }

        private bool IsClickedOnRoot(Vector2 mousePos)
        {
            if (_gridPartition.Query(_clickedNodeSearchRadius, mousePos).Count != 0)
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
            float distance = Vector2.Distance(mousePosition, closestNode.Position);
            foreach (RootNode node in rootNodes)
            {
                if (Vector2.Distance(node.Position, mousePosition) < distance)
                {
                    closestNode = node;
                    distance = Vector2.Distance(node.Position, mousePosition);
                }
            }
            return closestNode;
        }

    }
}
