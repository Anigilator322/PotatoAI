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
                if(IsPointInCircle(center,radius,(Vector2)obj.Transform.position))
                {
                    objectsInRadius.Add(obj);
                }
            }
            return objectsInRadius;
        }

        private static bool IsPointInCircle(Vector2 center, float radius, Vector2 point)
        {
            float distance = Vector2.Distance(center, point);
            return distance < radius;
        }
    }
}
