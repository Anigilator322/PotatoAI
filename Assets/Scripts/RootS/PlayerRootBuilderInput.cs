using Assets.Scripts.Map;
using Assets.Scripts.RootS.Metabolics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.RootS
{
    public class PlayerRootBuilderInput : MonoBehaviour
    {
        [SerializeField] private float _clickedNodeSearchRadius = 2f;

        [Inject] private PlayerInputActions _playerInputActions;
        [Inject] private GridPartition<RootNode> _gridPartition;
        [Inject] private RootBlueprintingSystem _rootBlueprintingSystem;
        [Inject] private RootGrowthSystem _rootGrowthSystem;
        [Inject] private MetabolicSystem _metabolicSystem;

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
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();
            Debug.Log("Mouse position: " + mousePos);
            Debug.Log("Worlds mousePos: " + Camera.main.ScreenToWorldPoint(mousePos));
            if (!_isDragging)
                return;
            Debug.Log("Drawing trajectory");
            //Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();
            DrawTrajectory(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            List<RootNode> queiriedNodes = _gridPartition.Query(_clickedNodeSearchRadius, mousePosition);
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);

            _currentBlueprint = _rootBlueprintingSystem.Create(_selectedType, _clickedNode);
        }

        private void DrawTrajectory(Vector2 mousePos)
        {
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            _currentBlueprint = _rootBlueprintingSystem.Update(_currentBlueprint, mousePos);
        }

        private void CancelBlueprinting()
        {
            if (_currentBlueprint == null)
                return;
            if (_metabolicSystem.IsAbleToBuild(_currentBlueprint))
                _rootGrowthSystem.StartGrowth(_currentBlueprint);
        }

        private bool IsClickedOnRoot(Vector2 mousePos)
        {
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
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
