using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilitySystem
    {
        private GridPartition<RootNode> _gridPartition;
        private int _cellSize;

        public VisibilitySystem(int cellSize)
        {
            _cellSize = cellSize;
        }

        private Vector2Int GetCellCoordinates(Vector2 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / _cellSize);
            return new Vector2Int(x, y);
        }

        private bool IsCellIntersectingCapsule(Vector2 cellCenter, Vector2 start, Vector2 end, float radius)
        {
            // Найти ближайшую точку от центра клетки на отрезке
            Vector2 segmentDir = end - start;
            float segmentLength = segmentDir.magnitude;
            segmentDir.Normalize();

            Vector2 toCell = cellCenter - start;
            float projection = Mathf.Clamp(Vector2.Dot(toCell, segmentDir), 0, segmentLength);

            Vector2 closestPoint = start + projection * segmentDir;

            // Проверить расстояние до ближайшей точки
            float distanceToCapsule = Vector2.Distance(cellCenter, closestPoint);

            return distanceToCapsule <= radius;
        }


        private List<Vector2Int> CapsuleCast(Vector2 start, Vector2 end, float radius)
        {
            //Search cells in capsule
            //Return List<Vector2Int> with all cells in capsule

            List<Vector2Int> cells = new List<Vector2Int>();

            // Определить AABB капсулы
            Vector2 minPoint = Vector2.Min(start, end) - new Vector2(radius, radius);
            Vector2 maxPoint = Vector2.Max(start, end) + new Vector2(radius, radius);

            // Преобразовать AABB в координаты сетки
            int minX = Mathf.FloorToInt(minPoint.x / _cellSize);
            int minY = Mathf.FloorToInt(minPoint.y / _cellSize);
            int maxX = Mathf.FloorToInt(maxPoint.x / _cellSize);
            int maxY = Mathf.FloorToInt(maxPoint.y / _cellSize);

            // Проверить каждую клетку в диапазоне AABB
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 cellCenter = new Vector2(x + 0.5f, y + 0.5f) * _cellSize;

                    if (IsCellIntersectingCapsule(cellCenter, start, end, radius))
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }

            return cells;
        }

        public void UpdateVisibilityForRootNode(RootNode revealer)
        {
            var edge = revealer.Position - revealer.Parent.Position;
            int revealRadius = ((int)revealer.Type);//Make configuration for revealRadius of different rootTypes
            float length = edge.magnitude;
            float width = revealRadius * 2;
            //var area = CapsuleCast()
            // Make BFS from revealer to all cells in capsule by quadrants. From 0 to 2PI
            // Make BFS from revealer.Parent to all cells in capsule by quadrants. From 0 to 2PI
            // Mark all gained cells as visible
            // Notify Cells about visibility change
        }

        public void UpdateVisibilityForResourcePoint()
        {

        }
    }
}
