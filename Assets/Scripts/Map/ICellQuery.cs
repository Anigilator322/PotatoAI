using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public interface ICellQuery<T> where T : PositionedObject
    {
        public Cell<T> QueryCellDirectly(Vector2 worldPosition);
        public List<Cell<T>> QueryCellsByCircle(float radius, Vector2 center);
        public List<Cell<T>> QueryCellsByRectangle(Vector2 size, Vector2 center);
        public List<Cell<T>> QueryCellsByCapsule(Vector2 start, Vector2 end, float radius);
    }
}
