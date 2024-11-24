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
        private RootBuildingPath _rootBuildingPath;
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
            _rootBuildingPath = new RootBuildingPath();
            if(_clickedNode.nextNodes.Count == 0)
            {
                _rootBuildingPath.IsNewProcess = false;
                _rootBuildingPath.AddInPath(_clickedNode.prevNode.Position);
                _rootBuildingPath.AddInPath(_clickedNode.Position);
            }
            else
            {
                _rootBuildingPath.IsNewProcess = true;
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
            _rootBuildingPath.AddInPath(position);
        }

        private void TryBlueprint()
        {
            if (!_isDragging)
                return;
            Debug.Log("Dragging");
            Vector2 mousePos = _playerInputActions.PlayerMap.MousePosition.ReadValue<Vector2>();

            if (Vector2.Distance(mousePos, _rootBuildingPath.RootPath[_rootBuildingPath.RootPath.Count-1]) > _distanceToBuildNewNode)
            {
                if(IsCreating(mousePos))
                {
                    CreatePathNode(mousePos);
                }
                else
                {
                    DecreasePath();
                }
                _rootBuildingPath.IsPathCorrect = CheckPathCorrection();
            }
        }

        private bool CheckPathCorrection()
        {
            int count = _rootBuildingPath.RootPath.Count;
            if (count < 2)
            {
                return true;
            }

            for (int i = 2; i < count; i++)
            {
                //Add rules
                Vector2 P1 = _rootBuildingPath.RootPath[i-2];
                Vector2 P2 = _rootBuildingPath.RootPath[i-1];
                Vector2 P3 = _rootBuildingPath.RootPath[i];


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
            _rootBuildingPath.RootPath.RemoveAt(_rootBuildingPath.RootPath.Count-1);
        }

        private void CreatePathNode(Vector2 mousePos)
        {
            _rootBuildingPath.RootPath.Add(mousePos);
        }

        private bool IsCreating(Vector2 mousePos)
        {
            Vector2 lastVector = (_rootBuildingPath.RootPath[_rootBuildingPath.RootPath.Count - 1] - _rootBuildingPath.RootPath[_rootBuildingPath.RootPath.Count - 2]).normalized;
            Vector2 newVector = (mousePos - _rootBuildingPath.RootPath[_rootBuildingPath.RootPath.Count - 1]).normalized;

            float dot = Vector2.Dot(lastVector, newVector);
            if(dot < 0)
            {
                return false;
            }    
            return true;
        }
    }
}
