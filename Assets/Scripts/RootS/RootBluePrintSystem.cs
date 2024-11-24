using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.RootS
{
    public class RootBluePrintSystem : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private GridPartition<RootNode> _gridPartition;

        private bool _isDragging = false;
        private float _clickedNodeSearchRadius = 2f;
        private float _distanceToBuildNewNode = 2f;
        private float _maxBuildAngle = 90f;
        private bool _isNewProcess;
        
        private List<Vector2> _buildingPath = new List<Vector2>();
        private bool _isPathCorrect;
        private bool isClickedOnRoot(Vector2 mousePos)
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
            _playerInputActions.PlayerMap.Enable();
            _playerInputActions.PlayerMap.LBMPressed.performed += _ => 
            { 
                if (isClickedOnRoot(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>()))
                {
                    _isDragging = true; 
                    PrepareBLuePrint(_playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>()); 
                } 
            };
            _playerInputActions.PlayerMap.LBMPressed.canceled += _ => { _isDragging = false; };
        }

        private void Update()
        {
            TryBlueprint();
        }

        private void PrepareBLuePrint(Vector2 mousePosition)
        {
            List<RootNode> quiredNodes = _gridPartition.Query(_clickedNodeSearchRadius,mousePosition);
            RootNode _clickedNode;
            _clickedNode = FindClosestNodeToMouse(quiredNodes, mousePosition);

            if(_clickedNode.nextNodes.Count == 0)
            {
                _isNewProcess = false;
                _buildingPath.Add(_clickedNode.prevNode.Position);
                _buildingPath.Add(_clickedNode.Position);
            }
            else
            {
                _isNewProcess=true;
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

        private void CreateNewPathNode(Vector2 position)
        {
            _buildingPath.Add(position);
        }

        private void TryBlueprint()
        {
            if (!_isDragging)
                return;
            Debug.Log("Dragging");
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();

            if (Vector2.Distance(mousePos, _buildingPath[_buildingPath.Count-1]) > _distanceToBuildNewNode)
            {
                if(IsCreating(mousePos))
                {
                    CreatePathNode(mousePos);
                }
                else
                {
                    DecreasePath();
                }
                _isPathCorrect = CheckPathCorrection();
            }
        }

        private bool CheckPathCorrection()
        {
            int count = _buildingPath.Count;
            if (count < 2)
            {
                return true;
            }

            for (int i = 2; i < count; i++)
            {
                //Add rules
                Vector2 P1 = _buildingPath[i-2];
                Vector2 P2 = _buildingPath[i-1];
                Vector2 P3 = _buildingPath[i];


                float cosTheta = Vector3.Dot(P2-P1, P3-P2);
                float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
                if (angle > _maxBuildAngle)
                {
                    return false;
                }
            }
            return true;
        }

        private void DecreasePath()
        {
            _buildingPath.RemoveAt(_buildingPath.Count-1);
        }

        private void CreatePathNode(Vector2 mousePos)
        {
            _buildingPath.Add(mousePos);
        }

        private bool IsCreating(Vector2 mousePos)
        {
            Vector2 lastVector = (_buildingPath[_buildingPath.Count - 1] - _buildingPath[_buildingPath.Count - 2]).normalized;
            Vector2 newVector = (mousePos - _buildingPath[_buildingPath.Count - 1]).normalized;

            float dot = Vector2.Dot(lastVector, newVector);
            if(dot < 0)
            {
                return false;
            }    
            return true;
        }
    }
}
