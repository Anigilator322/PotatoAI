using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem 
{ 
    public class RootsBlockSystem
    {
        private float _blockObjectsSeekRadius = 1f;

        private GridPartition<RootNode> _plantsRootsGridPartition;

        private bool IsBlock(Vector2 originPos, Vector2 targetPos)
        {
            var blockingObjects = CastSeekRadius(targetPos);

        }

        private IReadOnlyList<IPositionedObject> CastSeekRadius(Vector2 center)
        {
            var rootNodesList = _plantsRootsGridPartition.QueryByCircle(_blockObjectsSeekRadius, center);

            var result = new List<IPositionedObject>();
            result.AddRange(rootNodesList.Where(node => node.Type == RootType.Wall ).
                ToList<IPositionedObject>());

            return result;
        }

        
    }
}
