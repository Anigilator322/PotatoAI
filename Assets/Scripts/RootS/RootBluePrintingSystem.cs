using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using Zenject;

namespace Assets.Scripts.RootS
{
    public enum BlueprintingState
    {
        Idle,
        Creating,
        Removing,
        Canceling
    }
    public class RootBluePrintingSystem : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private GridPartition<RootNode> _gridPartition;

        private bool _isDragging = false;
        private BlueprintingState _currentState = BlueprintingState.Idle;
        private float _clickedNodeSearchRadius = 2f;
        private float _distanceToBuildNewNode = 2f;
        private float _maxBuildAngle = 90f;
        private float _minRemoveAngle = 90f;
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
            TryBlueprint();
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

        private void CreateNewPathNode(Vector2 position)
        {
            RootBuildingPath.AddInPath(position);
        }

        private void TryBlueprint()
        {
            if (!_isDragging)
                return;

            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();

            if (Vector2.Distance(mousePos, RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 1]) > _distanceToBuildNewNode)
            {
                float angle = CalculateAngleBetweenNodes(RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 2],
                            RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 1],
                            mousePos);
                if (IsCreating(mousePos))
                {
                    if (angle < _maxBuildAngle)
                    {
                        CreateNewPathNode(mousePos);
                    }
                    else
                    {
                        CreateNewPathNode(FindMaxAllowedPathNode(RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 2],
                            RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 1], angle));
                    }
                }
            }
        }

        private Vector2 FindMaxAllowedPathNode(Vector2 P1, Vector2 P2, float angle)
        {
            Vector2 v = (P2 - P1).normalized;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector2 rotatedV = new Vector2(v.x * Mathf.Cos(angleRad) - v.y * Mathf.Sin(angleRad),
                                           v.x * Mathf.Sin(angleRad) + v.y * Mathf.Cos(angleRad));
            Vector2 newNode = P2 + rotatedV * _distanceToBuildNewNode;
            return newNode;
        }

        public float CalculateAngleBetweenNodes(Vector2 node1, Vector2 node2, Vector2 newNode)
        {
            
            float cosTheta = Vector2.Dot((node2 - node1).normalized, (newNode - node2).normalized);
            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
            return angle;
        }

        private bool CheckAngleCorrecction(Vector2 node1, Vector2 node2, Vector2 newNode)
        {
            float angle = CalculateAngleBetweenNodes(node1, node2, newNode);
            if (angle > _maxBuildAngle)
            {
                return false;
            }
            else
                return true;
        }

        private bool CheckPathCorrection()
        {
            int count = RootBuildingPath.RootPath.Count;

            if (count < 2)
            {
                return true;
            }
            bool isCorrect = true;
            for (int i = 2; i < count; i++)
            {
                isCorrect = CheckAngleCorrecction(RootBuildingPath.RootPath[i - 2], RootBuildingPath.RootPath[i - 1], RootBuildingPath.RootPath[i]);
                if(!isCorrect)
                {
                    break;
                }
            }
            return isCorrect;
        }

        private void DecreasePath()
        {
            RootBuildingPath.RootPath.RemoveAt(RootBuildingPath.RootPath.Count-1);
        }

        private bool IsCreating(Vector2 mousePos)
        {
            Vector2 lastVector = (RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 1] - RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 2]).normalized;
            Vector2 newVector = (mousePos - RootBuildingPath.RootPath[RootBuildingPath.RootPath.Count - 1]).normalized;

            float dot = Vector2.Dot(lastVector, newVector);

            if(dot < 0)
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
