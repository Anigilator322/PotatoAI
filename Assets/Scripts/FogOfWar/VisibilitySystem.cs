﻿using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilitySystem
    {
        private PlantsModel _plantsModel;
        [Inject]
        ResourceDrawSystem _resourceDrawSystem;
        private Soil _soilResources;
        private int _cellSize;

        public int CellSize { get => _cellSize;}
        public float Radius { get; set; } = 1f;
        public List<Vector2> Starts { get; set; } = new List<Vector2>();
        public List<Vector2> Ends { get; set; } = new List<Vector2>();

        public Dictionary<Plant, List<IPositionedObject>> _visibleByPlantsPoints = new Dictionary<Plant, List<IPositionedObject>>();
        public VisibilitySystem(Soil soil, PlantsModel model)
        {
            _soilResources = soil;
            _plantsModel = model;
            _cellSize = 1;
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

        //Search cells in capsule
        //Return List<Vector2Int> with all cells in capsule
        //TODO: return List<PositionedObject> points that are in capsule
        private List<Vector2Int> CapsuleCast(Vector2 start, Vector2 end, float radius)
        {
            //DEBUG INFO
            Starts.Add(start);
            Ends.Add(end);
            //END DEBUG INFO
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

        public void UpdateVisibilityForRootNode(Plant plantOwner, RootNode revealer)
        {
            if (revealer.Parent is null)
                return;
            var edge = revealer.Transform.position - revealer.Parent.Transform.position;
            float revealRadius = ((int)revealer.Type + Radius);//Make configuration for revealRadius of different rootTypes
            float length = edge.magnitude;
            float width = revealRadius * 2;
            List<Vector2Int> area = CapsuleCast(revealer.Parent.Transform.position, revealer.Transform.position, revealRadius);
            CheckRoots(plantOwner, area);
            CheckResources(plantOwner, area);
        }

        private void CheckResources(Plant plantOwner, List<Vector2Int> area)
        {
            foreach (var cell in area)
            {
                
                if (!_visibleByPlantsPoints.ContainsKey(plantOwner))
                    _visibleByPlantsPoints.Add(plantOwner, new List<IPositionedObject>());
                var resources = _soilResources.GetResourcesFromCellDirectly(cell);
                foreach (var resource in resources)
                {
                    if (!_visibleByPlantsPoints[plantOwner].Contains(resource))
                    {
                        _visibleByPlantsPoints[plantOwner].Add(resource);
                    }
                }
            }
        }

        private void CheckRoots(Plant plantOwner, List<Vector2Int> area)
        {
            foreach (var cell in area)
            {
                foreach (var plant in _plantsModel.Plants) // foreach exist plants we check which roots are in capsule now
                {
                    if (!_visibleByPlantsPoints.ContainsKey(plant))
                        _visibleByPlantsPoints.Add(plant, new List<IPositionedObject>());
                    if (plant == plantOwner) // Skip nodes of the plant that is revealing
                        continue;
                    foreach (var node in plant.Roots.GetNodesFromCellDirectly(cell))
                    {
                        if (!_visibleByPlantsPoints[plant].Contains(node)) // If node is already visible, do not make recursive call again
                        {
                            _visibleByPlantsPoints[plant].Add(node);
                            UpdateVisibilityForRootNode(plant, node); // Reactive Update visibility for node that is visible now, to recalculate their own visibilities
                        }
                    }
                }
            }
        }
    }
}
