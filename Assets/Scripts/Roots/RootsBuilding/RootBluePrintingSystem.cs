using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public class RootBlueprintingSystem
    {
        public float _rootSegmentLength { get; private set; } = 0.8f;
        public float _maxBuildAngle { get; private set; } = 40f;
        private int _maxIters = 100;
        private int _curIters = 0;

        private void CreateNewPathNode(ScaffoldedRootBlueprint rootBlueprint, Vector2 direction)
        {
            Vector2 lastNodePosition = rootBlueprint.ScaffoldedPath[^1];
            direction.Normalize();
            rootBlueprint.AppendPoint(lastNodePosition + direction * _rootSegmentLength);
        }

        private bool TryBlueprint(ScaffoldedRootBlueprint rootBlueprint, Vector2 targetPos)
        {
            if(_curIters >= _maxIters)
                return false;
            _curIters++;
            Debug.Log("Trying to blueprint");
            
            if (Vector2.Distance(targetPos, rootBlueprint.ScaffoldedPath[^1]) <= _rootSegmentLength)
                return false;
            if(rootBlueprint.ScaffoldedPath.Count < 2)
            {
                CreateNewPathNode(rootBlueprint, targetPos);
                return true;
            }
            Vector2 lastPoint = rootBlueprint.ScaffoldedPath[^1];
            Vector2 secondLastPoint = rootBlueprint.ScaffoldedPath[^2];
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
                    Vector2 correctedPathNode = FindMaxAllowedPathNode(directionOfPath, directionToTarget);
                    CreateNewPathNode(rootBlueprint, directionToTarget);
                }
            }
            else
            {
                DecreasePath(rootBlueprint);
            }
            return true;
        }

        public ScaffoldedRootBlueprint Create(RootType type, RootNode parentNode)
        {
            return new ScaffoldedRootBlueprint(type, parentNode);
        }

        public ScaffoldedRootBlueprint Update(ScaffoldedRootBlueprint rootBlueprint, Vector2 targetPos)
        {
            _curIters = 0;
            while (TryBlueprint(rootBlueprint, targetPos)) ;

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

        private void DecreasePath(ScaffoldedRootBlueprint rootBlueprint)
        {
            rootBlueprint.RemoveLastPoint();
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
    }
}
