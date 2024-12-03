using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBlueprintingSystem
    {
        public float _distanceToBuildNewNode { get; private set; } = 2f;
        public float _maxBuildAngle { get; private set; } = 90f;

        private void CreateNewPathNode(RootBlueprint path,Vector2 targetVector, Vector2 lastNodePosition)
        {
            targetVector.Normalize();
            path.AddInPath((lastNodePosition + targetVector) * _distanceToBuildNewNode);
        }

        public bool TryBlueprint(RootBlueprint path, Vector2 targetPos)
        {
            if (Vector2.Distance(targetPos, path.RootPath[^1]) <= _distanceToBuildNewNode)
                return false;
            if(path.RootPath.Count < 2)
            {
                CreateNewPathNode(path,targetPos, path.RootPath[^1]);
                return true;
            }
            Vector2 lastPoint = path.RootPath[^1];
            Vector2 secondLastPoint = path.RootPath[^2];
            Vector2 directionToTarget = (targetPos - lastPoint).normalized;
            Vector2 directionOfPath = (lastPoint - secondLastPoint).normalized;
            
            if (IsCreating(directionOfPath, directionToTarget))
            {
                float angle = Vector2.Angle(directionToTarget, directionOfPath);
                if (angle < _maxBuildAngle)
                {
                    CreateNewPathNode(path, directionToTarget, lastPoint);
                }
                else
                {
                    Vector2 correctedPathNode = FindMaxAllowedPathNode(directionOfPath, directionToTarget);
                    CreateNewPathNode(path, directionToTarget, lastPoint);
                }
            }
            else
            {
                DecreasePath(path);
            }
            return true;
        }

        public RootBlueprint TryBlueprintWhileCan(RootType type, RootNode parentNode, Vector2 targetPos)
        {
            RootBlueprint rootBuildingPath = new RootBlueprint(type, parentNode);
            while (TryBlueprint(rootBuildingPath, targetPos)) ;

            return rootBuildingPath;
        }

        public RootBlueprint Create()

        public void UpdateBlueprint(RootBlueprint oldPath, Vector2 targetPos)
        {
            while (TryBlueprint(oldPath, targetPos)) ;
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

        public float CalculateAngleBetweenNodes(Vector2 node1, Vector2 node2, Vector2 newNode)
        {
            
            float cosTheta = Vector2.Dot((node2 - node1).normalized, (newNode - node2).normalized);
            float angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
            return angle;
        }

        private void DecreasePath(RootBlueprint path)
        {
            path.RootPath.RemoveAt(path.RootPath.Count-1);
        }

        private bool IsCreating(Vector2 path, Vector2 targetPos)
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
