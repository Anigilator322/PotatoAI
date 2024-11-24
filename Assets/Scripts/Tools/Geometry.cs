using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class Geometry
    {
        public List<PositionedObject> GetObjectsInRadius(Vector2 center, float radius, List<PositionedObject> objects)
        {
            List<PositionedObject> objectsInRadius = new List<PositionedObject>();
            foreach (PositionedObject obj in objects)
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
