using Assets.Scripts.Roots;
using Assets.Scripts.Tools;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
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

        public List<T> GetObjects()
        {
            return _positionedObjects;
        }
    }

    public class GridPartition<T> 
        where T : IPositionedObject
    {
        private int _cellSize;
        private Dictionary<Vector2Int, Cell<T>> _grid;

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
            Vector2Int cellCoordinates = GetCellCoordinates((Vector2)positionedObject.Transform.position);

            if (!_grid.ContainsKey(cellCoordinates))
            {
                _grid[cellCoordinates] = new Cell<T>(positionedObject);
            }
            else
            {
                _grid[cellCoordinates].AddObject(positionedObject);
            }
        }

        private List<T> GetPointsInCell(Vector2Int cellCoordinates)
        {
            if (_grid.ContainsKey(cellCoordinates))
            {
                return _grid[cellCoordinates].GetObjects();
            }

            return new List<T>();
        }

        private bool IsAnyCornerInRadius(Vector2Int cellCoordinates, Vector2 worldPosCenter, float radius)
        {
            Vector2 downLeft = new Vector2(cellCoordinates.x * _cellSize, cellCoordinates.y * _cellSize);
            Vector2 upRight = new Vector2(downLeft.x + _cellSize, downLeft.y + _cellSize);
            Vector2 upLeft = new Vector2(downLeft.x,downLeft.y + _cellSize);
            Vector2 downRight = new Vector2(downLeft.x + _cellSize, downLeft.y);

            if (Vector2.Distance(downLeft, worldPosCenter) < radius) return true;
            if (Vector2.Distance(upRight, worldPosCenter) < radius) return true;
            if (Vector2.Distance(downRight, worldPosCenter) < radius) return true;
            if (Vector2.Distance(upLeft,worldPosCenter)<radius) return true;

            return false;
        }

        public List<T> Query(Vector2 worldPosition)
        {
            Vector2Int cellCoordinates = GetCellCoordinates(worldPosition);
            return GetPointsInCell(cellCoordinates);
        }

        public List<T> Query(float radius, Vector2 worldPosCenter, bool strictSelection = true)
        {
            // ¬ычисл€ем диапазон €чеек дл€ проверки
            Vector2Int minCell = GetCellCoordinates(new Vector2(worldPosCenter.x - radius, worldPosCenter.y - radius));
            Vector2Int maxCell = GetCellCoordinates(new Vector2(worldPosCenter.x + radius, worldPosCenter.y + radius));

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
                        if (IsAnyCornerInRadius(cellCoordinates, worldPosCenter, radius))
                        {
                            positionedObjects.AddRange(_grid[cellCoordinates].GetObjects());
                        } 
                    }
                }
            }

            if(strictSelection)
                return Geometry.GetObjectsInRadius<T>(worldPosCenter, radius, positionedObjects);
            else
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
