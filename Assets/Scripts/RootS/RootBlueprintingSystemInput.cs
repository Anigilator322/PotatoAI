using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBlueprintingSystemInput: MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private bool _isDragging = false;
        private float _clickedNodeSearchRadius = 2f;
        private GridPartition<RootNode> _gridPartition;
        private RootBlueprintingSystem _rootBlueprintingSystem;
        private RootNode _clickedNode;
        private RootType _selectedType;

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

        private void CancelBlueprinting()
        {
            
        }

        void Update()
        {
            if (!_isDragging)
                return;
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();
            
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            _rootBlueprintingSystem.TryBlueprintWhileCan(_selectedType, _clickedNode, mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            List<RootNode> queiriedNodes = _gridPartition.Query(_clickedNodeSearchRadius, mousePosition);
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);
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
