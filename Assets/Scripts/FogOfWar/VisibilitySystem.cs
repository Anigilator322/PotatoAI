using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilitySystem
    {
        public CapsuleCutSystem CapsuleCutSystem { get; set; }
        public VisibilityModel VisibilityComponent { get; set; }

        private const int CELL_SIZE = 1;

        [Inject]
        public VisibilitySystem(PlantsModel model, Soil soil, Renderer fogOfWarRenderer)
        {
            VisibilityComponent = new VisibilityModel(model, soil);
            CapsuleCutSystem = new CapsuleCutSystem(fogOfWarRenderer);
        }

        public void Reset()
        {
            VisibilityComponent.Reset();
            CapsuleCutSystem.Reset();
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
            int minX = Mathf.FloorToInt(minPoint.x / CELL_SIZE);
            int minY = Mathf.FloorToInt(minPoint.y / CELL_SIZE);
            int maxX = Mathf.FloorToInt(maxPoint.x / CELL_SIZE);
            int maxY = Mathf.FloorToInt(maxPoint.y / CELL_SIZE);

            // Проверить каждую клетку в диапазоне AABB
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 cellCenter = new Vector2(x + 0.5f, y + 0.5f) * CELL_SIZE;

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
            VisibilityComponent.VisibilityCapsules.Add(capsule);
            //OnCapsuleCreated?.Invoke(capsule);
            CapsuleCutSystem.SetCapsule(capsule);
            CapsuleCutSystem.UpdateVisionShader();
            List<Vector2Int> area = CapsuleCast(capsule);
            CheckRoots(plantOwner, area);
            CheckResources(plantOwner, area);
            if(VisibilityComponent.PlantsModel.Plants.Count == 0)
                return;
        }

        private void CheckResources(Plant plantOwner, List<Vector2Int> area)
        {
            foreach (var cell in area)
            {
                
                if (!VisibilityComponent.VisibleByPlantsPoints.ContainsKey(plantOwner))
                    VisibilityComponent.VisibleByPlantsPoints.Add(plantOwner, new List<IPositionedObject>());
                var resources = VisibilityComponent.SoilResources.GetResourcesFromCellDirectly(cell);
                foreach (var resource in resources)
                {
                    if (!VisibilityComponent.VisibleByPlantsPoints[plantOwner].Contains(resource))
                    {
                        VisibilityComponent.VisibleByPlantsPoints[plantOwner].Add(resource);
                    }
                }
            }
        }

        private void CheckRoots(Plant plantOwner, List<Vector2Int> area)
        {
            foreach (var cell in area)
            {
                foreach (var plant in VisibilityComponent.PlantsModel.Plants) // foreach exist plants we check which roots are in capsule now
                {
                    if (!VisibilityComponent.VisibleByPlantsPoints.ContainsKey(plant))
                        VisibilityComponent.VisibleByPlantsPoints.Add(plant, new List<IPositionedObject>());
                    if (plant == plantOwner) // Skip nodes of the plant that is revealing
                        continue;
                    foreach (var node in plant.Roots.GetNodesFromCellDirectly(cell))
                    {
                        if (!VisibilityComponent.VisibleByPlantsPoints[plant].Contains(node)) // If node is already visible, do not make recursive call again
                        {
                            VisibilityComponent.VisibleByPlantsPoints[plant].Add(node);
                            UpdateVisibilityForRootNode(plant, node); // Reactive Update visibility for node that is visible now, to recalculate their own visibilities
                        }
                    }
                }
            }
        }
    }
}
