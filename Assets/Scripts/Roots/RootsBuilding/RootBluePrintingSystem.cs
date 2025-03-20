using Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class RootBlueprintingSystem
    {
        [Inject] 
        private RootsBlockSystem _rootBlockSystem;
        public float _rootSegmentLength { get; private set; } = 0.8f;
        public float _maxBuildAngle { get; private set; } = 15f;

        private void CreateNewPathNode(DrawingRootBlueprint rootBlueprint, Vector2 direction)
        {
            Vector2 lastNodePosition = rootBlueprint.RootPath[^1];
            direction.Normalize();
            var point = lastNodePosition + direction * _rootSegmentLength;
            rootBlueprint.AppendPoint(point);
            SetIsBlockedAfterActions(rootBlueprint, BlueprintingResult.Incr);
        }

        enum BlueprintingResult { Incr = 1, Decr = -1, Unchanged = 0};

        private BlueprintingResult TryBlueprint(DrawingRootBlueprint rootBlueprint, Vector2 targetPos)
        {
            if (Vector2.Distance(targetPos, rootBlueprint.RootPath[^1]) <= _rootSegmentLength)
                return BlueprintingResult.Unchanged;

            if(rootBlueprint.RootPath.Count < 2)
            {
                CreateNewPathNode(rootBlueprint, targetPos - rootBlueprint.RootPath[^1]);
                return BlueprintingResult.Incr;
            }

            Vector2 lastPoint = rootBlueprint.RootPath[^1];
            Vector2 secondLastPoint = rootBlueprint.RootPath[^2];
            Vector2 directionToTarget = (targetPos - lastPoint).normalized;
            Vector2 directionOfPath = (lastPoint - secondLastPoint).normalized;
            
            if (IsCoDirected(directionOfPath, directionToTarget))
            {
                float angle = Vector2.Angle(directionToTarget, directionOfPath);
                if (angle < _maxBuildAngle)
                {
                    CreateNewPathNode(rootBlueprint, directionToTarget);
                }
                else
                {
                    Vector2 correctedDirection = FindMaxAllowedPathNode(directionOfPath, directionToTarget);
                    rootBlueprint.AppendPoint(rootBlueprint.RootPath[^1] + correctedDirection);
                    SetIsBlockedAfterActions(rootBlueprint, BlueprintingResult.Incr);
                }
                return BlueprintingResult.Incr;
            }
            else
            {
                var result = TryDecreasePath(rootBlueprint) ? BlueprintingResult.Decr : BlueprintingResult.Unchanged;
                SetIsBlockedAfterActions(rootBlueprint, result);
                return result;
            }
        }

        public DrawingRootBlueprint Create(RootType type, RootNode parentNode)
        {
            return new DrawingRootBlueprint(type, parentNode);
        }

        public DrawingRootBlueprint Update(DrawingRootBlueprint rootBlueprint, Vector2 targetPos, int maxIters = 3)
        {
            BlueprintingResult firstResult = TryBlueprint(rootBlueprint, targetPos);

            if (firstResult == BlueprintingResult.Unchanged) 
                return rootBlueprint;

            BlueprintingResult result;
            while (firstResult == (result = TryBlueprint(rootBlueprint, targetPos)));

            if (result == BlueprintingResult.Unchanged
                || result == BlueprintingResult.Incr)
                return rootBlueprint;

            result = TryBlueprint(rootBlueprint, targetPos);

            return rootBlueprint;
        }

        private Vector2 RotateVector(Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        private Vector2 FindMaxAllowedPathNode(Vector2 directionOfPath, Vector2 directionToTarget)
        {
            float sign = Mathf.Sign(Vector2.SignedAngle(directionOfPath, directionToTarget));
            Vector2 correctedDirection = RotateVector(directionOfPath, sign * _maxBuildAngle);
            return correctedDirection;
        }

        private float CalculateAngleBetweenNodes(Vector2 node1, Vector2 node2, Vector2 newNode)
        {
            float cosTheta = Vector2.Dot((node2 - node1).normalized, (newNode - node2).normalized);
            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
            return angle;
        }

        private bool TryDecreasePath(IRootBlueprint rootBlueprint)
        {
            return rootBlueprint.TryRemoveLastPoint();
        }

        private bool IsCoDirected(Vector2 path, Vector2 targetPos)
        {
            Vector2 projection = Vector2.Dot(targetPos, path) / Vector2.Dot(path, path) * path;
            float dot = Vector2.Dot(projection, path);
            if (dot < 0)
            {
                return false;
            }    
            return true;
        }

        private void SetIsBlockedAfterActions(DrawingRootBlueprint rootBlueprint, BlueprintingResult actionsResult)
        {
            if(actionsResult == BlueprintingResult.Decr)
            {
                rootBlueprint.IsBlocked = _rootBlockSystem.IsAnyBlock(rootBlueprint);
            }
            if (actionsResult == BlueprintingResult.Incr)
            {
                if (rootBlueprint.RootPath.Count < 2)
                { 
                    rootBlueprint.IsBlocked = false;
                    return;
                }
                var result = _rootBlockSystem.IsBlock(rootBlueprint.RootPath[^1], rootBlueprint.RootPath[^2]);
                if(rootBlueprint.IsBlocked == false)
                    rootBlueprint.IsBlocked = result;

            }
        }
    }
}
