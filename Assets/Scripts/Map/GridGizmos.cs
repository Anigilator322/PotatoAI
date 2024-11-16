using Assets.Scripts.RootS;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Map
{
    public class GridGizmos : MonoBehaviour
    {
        [Inject]public GridPartition<RootSystem> _grid;


        private void Start()
        {
            
        }
        void OnDrawGizmos()
        {
            if (_grid == null) return;

            // Визуализация ячеек, в которых есть точки
            Gizmos.color = Color.gray;

            // Перебираем все существующие ячейки, в которых есть точки
            foreach (var cell in _grid.GetAllCellsWithPoints())
            {
                Vector2Int cellCoordinates = cell.Key;  // Координаты ячейки в сетке

                // Рассчитываем мировые координаты левого нижнего угла ячейки
                Vector3 cellWorldPos = new Vector3(cellCoordinates.x * _grid.GetCellSize(), cellCoordinates.y * _grid.GetCellSize(), 0);

                // Рисуем границы ячейки
                Gizmos.DrawLine(cellWorldPos, cellWorldPos + new Vector3(_grid.GetCellSize(), 0, 0));   // Нижняя граница
                Gizmos.DrawLine(cellWorldPos, cellWorldPos + new Vector3(0, _grid.GetCellSize(), 0));   // Левая граница
                Gizmos.DrawLine(cellWorldPos + new Vector3(_grid.GetCellSize(), 0, 0), cellWorldPos + new Vector3(_grid.GetCellSize(), _grid.GetCellSize(), 0)); // Правая граница
                Gizmos.DrawLine(cellWorldPos + new Vector3(0, _grid.GetCellSize(), 0), cellWorldPos + new Vector3(_grid.GetCellSize(), _grid.GetCellSize(), 0)); // Верхняя граница

                

                Gizmos.color = Color.gray;  // Возвращаем цвет к серому для следующей ячейки
            }
        }

    }
}
