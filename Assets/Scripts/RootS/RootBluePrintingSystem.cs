using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBluePrintingSystem : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private GridPartition<RootNode> _gridPartition;

        private bool _isDragging = false;
        private float _clickedNodeSearchRadius = 2f;
        private float _distanceToBuildNewNode = 2f;
        private float _maxBuildAngle = 90f;
        public RootBuildingPath RootBuildingPath;

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

        private void Start()
        {
            _playerInputActions.PlayerMap.LBMPressed.performed += _ => 
            { 
                if (IsClickedOnRoot(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>()))
                {
                    _isDragging = true; 
                    PrepareBlueprint(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>()); 
                } 
            };
            _playerInputActions.PlayerMap.LBMPressed.canceled += _ => { _isDragging = false; CancelBluePrinting(); };
        }

        private void Update()
        {
            if (!_isDragging)
                return;
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();
            if (mousePos.magnitude / _distanceToBuildNewNode < 10)
                TryBlueprint(mousePos);
            else
                TryBlueprintWhileCan(mousePos);
        }

        private void PrepareBlueprint(Vector2 mousePosition)
        {
            RootBuildingPath = new RootBuildingPath();
            List<RootNode> queiriedNodes = _gridPartition.Query(_clickedNodeSearchRadius, mousePosition);
            RootNode _clickedNode;
            _clickedNode = FindClosestNodeToMouse(queiriedNodes, mousePosition);
            RootBuildingPath.IsNewProcess = _clickedNode.nextNodes.Count != 0;
            RootBuildingPath.AddInPath(_clickedNode.prevNode.Position);
            RootBuildingPath.AddInPath(_clickedNode.Position);
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

        private void CreateNewPathNode(Vector2 newNodePosition, Vector2 lastNodePosition)
        {
            newNodePosition.Normalize();
            RootBuildingPath.AddInPath((lastNodePosition + newNodePosition) * _distanceToBuildNewNode);
        }

        private bool TryBlueprint(Vector2 mousePos)
        {
            if (Vector2.Distance(mousePos, RootBuildingPath.RootPath[^1]) <= _distanceToBuildNewNode)
                return false;
            if(RootBuildingPath.RootPath.Count < 2)
            {
                CreateNewPathNode(mousePos, RootBuildingPath.RootPath[^1]);
                return true;
            }
            Vector2 lastPoint = RootBuildingPath.RootPath[^1];
            Vector2 secondLastPoint = RootBuildingPath.RootPath[^2];
            Vector2 directionToMouse = (mousePos - lastPoint).normalized;
            Vector2 directionOfPath = (lastPoint - secondLastPoint).normalized;
            
            if (IsCreating(directionOfPath,directionToMouse))
            {
                float angle = Vector2.Angle(directionToMouse, directionOfPath);
                if (angle < _maxBuildAngle)
                {
                    CreateNewPathNode(directionToMouse,lastPoint);
                }
                else
                {
                    Vector2 correctedPathNode = FindMaxAllowedPathNode(directionOfPath, directionToMouse);
                    CreateNewPathNode(directionToMouse, lastPoint);
                }
            }
            else
            {
                DecreasePath();
            }
            return true;
        }

        private void TryBlueprintWhileCan(Vector2 mousePos)
        {
            while (TryBlueprint(mousePos)) ;
        }

        private Vector2 RotateVector(Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);

        }

        private Vector2 FindMaxAllowedPathNode(Vector2 directionOfPath, Vector2 directionToMouse)
        {
            float sign = Mathf.Sign(Vector2.SignedAngle(directionOfPath, directionToMouse));
            Vector2 correctedDirection = RotateVector(directionOfPath, sign * _maxBuildAngle);
            return correctedDirection;
        }

        public float CalculateAngleBetweenNodes(Vector2 node1, Vector2 node2, Vector2 newNode)
        {
            
            float cosTheta = Vector2.Dot((node2 - node1).normalized, (newNode - node2).normalized);
            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
            return angle;
        }

        private void DecreasePath()
        {
            RootBuildingPath.RootPath.RemoveAt(RootBuildingPath.RootPath.Count-1);
        }

        private bool IsCreating(Vector2 path, Vector2 mousePos)
        {
            Vector2 projection = Vector2.Dot(mousePos, path) / Vector2.Dot(path, path) * path;
            float dot = Vector2.Dot(projection, path);
            if (dot < 0)
            {
                return false;
            }    
            return true;
        }

        /// <summary>
        /// RootGrowthSystem.GrowRoot(_rootBuildingPath);
        /// </summary>
        private void CancelBluePrinting()
        {
            
        }
    }
}
