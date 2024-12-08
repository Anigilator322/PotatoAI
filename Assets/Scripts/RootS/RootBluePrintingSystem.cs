using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBlueprintingSystem
    {
        public float _rootSegmentLength { get; private set; } = 2f;
        public float _maxBuildAngle { get; private set; } = 90f;

        private void CreateNewPathNode(RootBlueprint rootBlueprint, Vector2 targetDirection)
        {
            Vector2 lastNodePosition = rootBlueprint.RootPath[^1];
            targetDirection.Normalize();
            rootBlueprint.AddInPath(targetDirection + lastNodePosition * _rootSegmentLength);
        }

        private bool TryBlueprint(RootBlueprint rootBlueprint, Vector2 targetPos)
        {
            if (Vector2.Distance(targetPos, rootBlueprint.RootPath[^1]) <= _rootSegmentLength)
                return false;
            if(rootBlueprint.RootPath.Count < 2)
            {
                CreateNewPathNode(rootBlueprint,targetPos);
                return true;
            }
            Vector2 lastPoint = rootBlueprint.RootPath[^1];
            Vector2 secondLastPoint = rootBlueprint.RootPath[^2];
            Vector2 directionToTarget = (targetPos - lastPoint).normalized;
            Vector2 directionOfPath = (lastPoint - secondLastPoint).normalized;
            
            if (IsCodirected(directionOfPath, directionToTarget))
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

        public RootBlueprint Create(RootType type, RootNode parentNode)
        {
            RootBlueprint rootBlueprint = new RootBlueprint(type, parentNode);

            return rootBlueprint;
        }

        public RootBlueprint Update(RootBlueprint rootBlueprint, Vector2 targetPos)
        {
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

        private void DecreasePath(RootBlueprint rootBlueprint)
        {
            rootBlueprint.RootPath.RemoveAt(rootBlueprint.RootPath.Count-1);
        }

        private bool IsCodirected(Vector2 path, Vector2 targetPos)
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
