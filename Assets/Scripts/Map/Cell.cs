using System.Collections.Generic;
namespace Assets.Scripts.Map
{
    public class Cell<T> where T : PositionedObject
    {
        private List<T> _positionedObjects;
        public Cell()
        {
            _positionedObjects = new List<T>();
        }
        public Cell(T point)
        {
            _positionedObjects = new List<T>();
            _positionedObjects.Add(point);
        }
        public void AddIndex(T point)
        {
            _positionedObjects.Add(point);
        }
        public List<T> GetIndexes()
        {
            return _positionedObjects;
        }
    }
}
