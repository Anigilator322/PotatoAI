using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
namespace Assets.Scripts.Map
{
    public class Cell<T>
    {
        private List<int> _indexes;
        public Cell()
        {
            _indexes = new List<int>();
        }
        public Cell(int index)
        {
            _indexes = new List<int>();
            _indexes.Add(index);
        }
        public void AddIndex(int index)
        {
            _indexes.Add(index);
        }
        public List<int> GetIndexes()
        {
            return _indexes;
        }
    }

    public class GridPartition<T> 
        where T : IPositionedObjects
    {
        private int _cellSize;
        private Dictionary<Vector2Int, Cell<T>> _grid;
        private T _positionedObjects;
        public GridPartition(int cellSize, T positionedObjects)
        {
            _positionedObjects = positionedObjects;
            _cellSize = cellSize;
            _grid = new Dictionary<Vector2Int, Cell<T>>();
        }

        private Vector2Int GetCellCoordinates(Vector2 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / _cellSize);
            return new Vector2Int(x, y);    
        }


        public void Insert(int index)
        {

            Vector2Int cellCoordinates = GetCellCoordinates(_positionedObjects.GetPositionById(index));

            if (!_grid.ContainsKey(cellCoordinates))
            {
                _grid[cellCoordinates] = new Cell<T>(index);
            }
            else
            {
                _grid[cellCoordinates].AddIndex(index);
            }
        }
        private List<int> GetPointsInCell(Vector2Int cellCoordinates)
        {
            if (_grid.ContainsKey(cellCoordinates))
            {
                return _grid[cellCoordinates].GetIndexes();
            }

            return new List<int>();
        }

        

        public List<int> Query(Vector2 worldPosition)
        {
            Vector2Int cellCoordinates = GetCellCoordinates(worldPosition);
            return GetPointsInCell(cellCoordinates);
        }
        public List<int> Query(float radius, Vector2 center)
        {

            // Вычисляем диапазон ячеек для проверки
            Vector2Int minCell = GetCellCoordinates(new Vector2(center.x - radius, center.y - radius));
            Vector2Int maxCell = GetCellCoordinates(new Vector2(center.x + radius, center.y + radius));

            List<int> indexes = new List<int>();

            // Перебираем все ячейки в этом диапазоне
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(x, y);

                    // Если ячейка существует
                    if (_grid.ContainsKey(cellCoordinates))
                    {
                        // Проверяем каждую точку в ячейке
                        indexes.AddRange(_grid[cellCoordinates].GetIndexes());
                    }
                }
            }

            return indexes;
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
