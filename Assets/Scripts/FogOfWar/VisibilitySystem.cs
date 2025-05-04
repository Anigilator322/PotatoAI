using Assets.Scripts.Bootstrap;
using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Tools;
using Assets.Scripts.UX;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using Zenject;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilitySystem
    {
        private PlantsModel _plantsModel;

        private Soil _soilResources;
        private int _cellSize;

        public int CellSize { get => _cellSize;}
        public List<VisibilityCapsule> VisibilityCapsules { get; set; } = new List<VisibilityCapsule>();

        public Dictionary<Plant, List<IPositionedObject>> VisibleByPlantsPoints = new Dictionary<Plant, List<IPositionedObject>>();
        public CapsuleCutSystem _capsuleCutSystem { get; set; }

        public Action<VisibilityCapsule> OnCapsuleCreated;
        public VisibilitySystem(Soil soil, PlantsModel model, ResourcePointsConfig resourcePointsConfig)
        {
            _soilResources = soil;
            _plantsModel = model;
            _cellSize = (int)resourcePointsConfig.size;
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
        private List<Vector2Int> CapsuleCast(VisibilityCapsule capsule)
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            // Определить AABB капсулы
            Vector2 minPoint = Vector2.Min(capsule.Start, capsule.End) - new Vector2(capsule.Radius, capsule.Radius);
            Vector2 maxPoint = Vector2.Max(capsule.Start, capsule.End) + new Vector2(capsule.Radius, capsule.Radius);

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

                    if (IsCellIntersectingCapsule(cellCenter, capsule.Start, capsule.End, capsule.Radius))
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

            float revealRadius = (revealer.Type switch
            {
                RootType.Recon => 2,
                RootType.Wall => 0.5f,
                _ => 1
            });                

            float length = edge.magnitude;
            float width = revealRadius * 2;
            var capsule = new VisibilityCapsule(revealer.Parent.Transform.position, revealer.Transform.position, revealRadius);
            VisibilityCapsules.Add(capsule);
            OnCapsuleCreated?.Invoke(capsule);
            List<Vector2Int> area = CapsuleCast(capsule);
            CheckRoots(plantOwner, area);
            CheckResources(plantOwner, area);
        }

        private void CheckResources(Plant plantOwner, List<Vector2Int> area)
        {
            foreach (var cell in area)
            {
                
                if (!VisibleByPlantsPoints.ContainsKey(plantOwner))
                    VisibleByPlantsPoints.Add(plantOwner, new List<IPositionedObject>());
                var resources = _soilResources.GetResourcesFromCellDirectly(cell);
                foreach (var resource in resources)
                {
                    if (!VisibleByPlantsPoints[plantOwner].Contains(resource))
                    {
                        VisibleByPlantsPoints[plantOwner].Add(resource);
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
                    if (!VisibleByPlantsPoints.ContainsKey(plant))
                        VisibleByPlantsPoints.Add(plant, new List<IPositionedObject>());
                    if (plant == plantOwner) // Skip nodes of the plant that is revealing
                        continue;
                    foreach (var node in plant.Roots.GetNodesFromCellDirectly(cell))
                    {
                        if (!VisibleByPlantsPoints[plant].Contains(node)) // If node is already visible, do not make recursive call again
                        {
                            VisibleByPlantsPoints[plant].Add(node);
                            UpdateVisibilityForRootNode(plant, node); // Reactive Update visibility for node that is visible now, to recalculate their own visibilities
                        }
                    }
                }
            }
        }
    }
}
