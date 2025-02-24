using System.Collections.Generic;
namespace Assets.Scripts.Map
{
    public class Cell<T> where T : IPositionedObject
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

        public void AddObject(T point)
        {
            _positionedObjects.Add(point);
        }

        public void RemoveObject(T point)
        {
            _positionedObjects.Remove(point);
        }

        public List<T> GetObjects()
        {
            return _positionedObjects;
        }
    }

}
