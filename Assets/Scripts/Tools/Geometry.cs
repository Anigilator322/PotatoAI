using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class Geometry
    {
        public static List<T> GetObjectsInRadius<T>(Vector2 center, float radius, List<T> objects)
            where T : IPositionedObject
        {
            List<T> objectsInRadius = new List<T>();
            foreach (T obj in objects)
            {
                if (IsPointInCircle(center, radius, (Vector2)obj.Transform.position))
                {
                    objectsInRadius.Add(obj);
                }
            }
            return objectsInRadius;
        }

        public static List<T> GetObjectsInRectangle<T>(List<T> objcets, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
            where T : IPositionedObject
        {
            List<T> objectsInRectangle = new List<T>();
            foreach (T obj in objcets)
            {
                if (IsPointInRectangle((Vector2)obj.Transform.position, a, b, c, d))
                {
                    objectsInRectangle.Add(obj);
                }
            }
            return objectsInRectangle;
        }

        private static bool IsPointInCircle(Vector2 center, float radius, Vector2 point)
        {
            float distance = Vector2.Distance(center, point);
            return distance < radius;
        }

        private static bool IsPointInRectangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float CrossProduct(Vector2 v1, Vector2 v2)
            {
                return v1.x * v2.y - v1.y * v2.x;
            }

            bool IsSameSide(Vector2 p, Vector2 edgeStart, Vector2 edgeEnd)
            {
                Vector2 edge = edgeEnd - edgeStart;
                Vector2 toP = p - edgeStart;
                Vector2 toA = a - edgeStart;

                float cross1 = CrossProduct(edge, toP);
                float cross2 = CrossProduct(edge, toA);

                return cross1 * cross2 >= 0;
            }

            return IsSameSide(point, a, b) &&
                   IsSameSide(point, b, c) &&
                   IsSameSide(point, c, d) &&
                   IsSameSide(point, d, a);
        }
    }
}
