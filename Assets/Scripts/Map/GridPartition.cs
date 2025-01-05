using Assets.Scripts.Roots;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
namespace Assets.Scripts.Map
{

    public class GridPartition<T> : IObjectsQuery<T>
        where T : PositionedObject
    {
        private int _cellSize;
        private Dictionary<Vector2Int, Cell<T>> _grid;
        //private List<T> _positionedObjects = new List<T>();

        public GridPartition(int cellSize)
        { 
            _cellSize = cellSize;
            _grid = new Dictionary<Vector2Int, Cell<T>>();
        }

        private Vector2Int GetCellCoordinates(Vector2 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / _cellSize);
            return new Vector2Int(x, y);    
        }


        public void Insert(T positionedObject)
        {

            Vector2Int cellCoordinates = GetCellCoordinates(positionedObject.Position);

            if (!_grid.ContainsKey(cellCoordinates))
            {
                _grid[cellCoordinates] = new Cell<T>(positionedObject);
            }
            else
            {
                _grid[cellCoordinates].AddIndex(positionedObject);
            }
        }
        private List<T> GetPointsInCell(Vector2Int cellCoordinates)
        {
            if (_grid.ContainsKey(cellCoordinates))
            {
                return _grid[cellCoordinates].GetIndexes();
            }

            return new List<T>();
        }

        private bool isAnyCornerInRadius(Vector2Int cellCoordinates, Vector2 center, float radius)
        {
            Vector2 downLeft = new Vector2(cellCoordinates.x * _cellSize, cellCoordinates.y * _cellSize);
            Vector2 upRight = new Vector2(downLeft.x + _cellSize, downLeft.y + _cellSize);
            Vector2 upLeft = new Vector2(downLeft.x,downLeft.y + _cellSize);
            Vector2 downRight = new Vector2(downLeft.x + _cellSize, downLeft.y);

            if (Vector2.Distance(downLeft, center) < radius) return true;
            if (Vector2.Distance(upRight, center) < radius) return true;
            if (Vector2.Distance(downRight, center) < radius) return true;
            if (Vector2.Distance(upLeft,center)<radius) return true;

            return false;
        }

        public List<T> QueryDirectly(Vector2 worldPosition)
        {
            Vector2Int cellCoordinates = GetCellCoordinates(worldPosition);
            return GetPointsInCell(cellCoordinates);
        }

        public List<T> QueryByCircle(float radius, Vector2 center)
        {

            // ¬ычисл€ем диапазон €чеек дл€ проверки
            Vector2Int minCell = GetCellCoordinates(new Vector2(center.x - radius, center.y - radius));
            Vector2Int maxCell = GetCellCoordinates(new Vector2(center.x + radius, center.y + radius));

            List<T> positionedObjects = new List<T>();

            // ѕеребираем все €чейки в этом диапазоне
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(x, y);

                    // ≈сли €чейка существует
                    if (_grid.ContainsKey(cellCoordinates))
                    {
                        if (isAnyCornerInRadius(cellCoordinates, center, radius))
                        {
                            positionedObjects.AddRange(_grid[cellCoordinates].GetIndexes());
                        } 
                    }
                }
            }

            return positionedObjects;
        }
        public Dictionary<Vector2Int, Cell<T>> GetAllCellsWithPoints()
        {
            return _grid;
        }
        public int GetCellSize()
        {
            return _cellSize;
        }
    }
}
