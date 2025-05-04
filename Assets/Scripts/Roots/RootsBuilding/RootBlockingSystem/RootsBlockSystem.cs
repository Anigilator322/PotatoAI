using Assets.Scripts.Map;
using Assets.Scripts.Roots.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem 
{ 
    public class RootsBlockSystem
    {
        private float _blockObjectsSeekRadius = 2f;

        private PlantsModel _plantsModel;

        public RootsBlockSystem(PlantsModel plantsModel, float blockObjectsSeekRadius)
        {
            _plantsModel = plantsModel;
            _blockObjectsSeekRadius = blockObjectsSeekRadius;
        }

        public bool IsAnyBlock(DrawingRootBlueprint rootBlueprint)
        {
            var rootPath = rootBlueprint.RootPath;
            if(rootPath is null || rootPath.Count < 2)
            {
                return false;
            }

            for (int i = 0; i < rootPath.Count - 1; i++)
            {
                if (IsBlocked(rootPath[i], rootPath[i + 1]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsBlocked(Vector2 originPos, Vector2 targetPos)
        {
            var blockingObjects = GetBlockingObjects(targetPos);
            var result = IsAnyIntersections(blockingObjects, originPos, targetPos);
            return result;
        }

        private IReadOnlyList<IBlockerNode> GetBlockingObjects(Vector2 center)
        {
            var rootNodesList = new List<RootNode>();
            foreach (var plant in _plantsModel.Plants)
            {
                rootNodesList.AddRange(plant.Roots.GetNodesFromCircle(_blockObjectsSeekRadius, center));
            }

            var result = new List<IBlockerNode>();
            result.AddRange(rootNodesList.Where(node => node.Type == RootType.Wall ).
                ToList<IBlockerNode>());

            return result;
        }

        private bool IsAnyIntersections(IReadOnlyList<IBlockerNode> blockingObjects, Vector2 originPos, Vector2 targetPos)
        {
            Dictionary<IBlockerNode, bool> checkedNodes = new Dictionary<IBlockerNode, bool>();
            foreach (var blockingObject in blockingObjects)
            {
                if (!checkedNodes.ContainsKey(blockingObject))
                {
                    checkedNodes.Add(blockingObject, true);

                    var currentNode = blockingObject.Parent;
                    int i = 0;
                    while (i < blockingObject.Childs.Count)
                    {
                        if (IsIntersection(originPos, targetPos, blockingObject.Transform.position, currentNode.Transform.position))
                        {
                            return true;
                        }
                        currentNode = blockingObject.Childs[i];
                        i++;
                    }
                }
            }
            return false;
        }

        private bool IsIntersection(Vector2 originPos, Vector2 targetPos, Vector2 originPos2, Vector2 targetPos2)
        {
            return DoLinesIntersect(originPos, targetPos, originPos2, targetPos2);
        }

        private bool DoLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float d1 = Direction(p3, p4, p1);
            float d2 = Direction(p3, p4, p2);
            float d3 = Direction(p1, p2, p3);
            float d4 = Direction(p1, p2, p4);

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
            {
                return true;
            }

            if (d1 == 0 && OnSegment(p3, p4, p1)) return true;
            if (d2 == 0 && OnSegment(p3, p4, p2)) return true;
            if (d3 == 0 && OnSegment(p1, p2, p3)) return true;
            if (d4 == 0 && OnSegment(p1, p2, p4)) return true;

            return false;
        }

        private float Direction(Vector2 pi, Vector2 pj, Vector2 pk)
        {
            return (pk.x - pi.x) * (pj.y - pi.y) - (pk.y - pi.y) * (pj.x - pi.x);
        }

        private bool OnSegment(Vector2 pi, Vector2 pj, Vector2 pk)
        {
            return Mathf.Min(pi.x, pj.x) < pk.x && pk.x < Mathf.Max(pi.x, pj.x) &&
                   Mathf.Min(pi.y, pj.y) < pk.y && pk.y < Mathf.Max(pi.y, pj.y);
        }
    }
}
