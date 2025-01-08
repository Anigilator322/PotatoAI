using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class Geometry
    {
        public List<IPositionedObject> GetObjectsInRadius(Vector2 center, float radius, List<IPositionedObject> objects)
        {
            List<IPositionedObject> objectsInRadius = new List<IPositionedObject>();
            foreach (IPositionedObject obj in objects)
            {
                if(IsPointInCircle(center,radius,obj.Position))
                {
                    objectsInRadius.Add(obj);
                }
            }
            return objectsInRadius;
        }

        private bool IsPointInCircle(Vector2 center, float radius, Vector2 point)
        {
            float distance = Vector2.Distance(center, point);
            return distance < radius;
        }
    }
}
