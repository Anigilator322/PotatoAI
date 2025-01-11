using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public interface IObjectsQuery<T> where T : IPositionedObject
    {
        public List<T> QueryDirectlyCell(Vector2Int worldPosition);
        public List<T> QueryByCircle(float radius, Vector2 worldPosCenter, bool strictSelection=true);
    }
}
