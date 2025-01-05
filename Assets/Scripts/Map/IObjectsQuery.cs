using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public interface IObjectsQuery<T> where T : PositionedObject
    {
        public List<T> QueryDirectly(Vector2 worldPosition);
        public List<T> QueryByCircle(float radius, Vector2 center);
    }
}
