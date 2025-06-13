using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class SceneryAgent
    {
        private RootBlueprintingSystem _blueprintingSystem;
        private RootGrowthSystem _rootGrowthSystem;
        private PlantsModel _plantsModel;
        private SynchronizationContext _mainThreadContext;

        public Queue<List<Vector2>> Paths { get; private set; } = new Queue<List<Vector2>>();

        public SceneryAgent(RootBlueprintingSystem blueprintingSystem, RootGrowthSystem rootGrowthSystem, PlantsModel plantsModel)
        {
            _mainThreadContext = System.Threading.SynchronizationContext.Current;
            _blueprintingSystem = blueprintingSystem;
            _rootGrowthSystem = rootGrowthSystem;
            _plantsModel = plantsModel;
            Debug.Log(rootGrowthSystem.GetType().Name + " initialized for SceneryAgent.");
            var demoPaths = RootPathsDemo.GetDemoPaths();
            foreach (var path in demoPaths)
            {
                Paths.Enqueue(path);
            }
        }

        public async void Init()
        {
            Debug.Log("SceneryAgent initialized with " + Paths.Count + " paths.");
            await Execute();
        }

        private async Task Execute()
        {
            var baseRoot = _plantsModel.Plants[0].Roots.Nodes[0];
            while (Paths.Count > 0)
            {
                List<Vector2> path = Paths.Dequeue();
                DrawingRootBlueprint drawingRoot = null;
                _mainThreadContext.Send(_ => DrawingRootBlueprint.Create(RootType.Harvester, baseRoot), null);
                foreach (var point in path)
                {
                    _blueprintingSystem.Update(drawingRoot, point);
                }
                _rootGrowthSystem.StartGrowth(drawingRoot.blueprint);
                Debug.Log($"Path with {path.Count} points added to the growth system for root type {drawingRoot.RootType}.");
                await Task.Delay(1000);
            }
        }
    }

    // Пример инициализации путей для SceneryAgent
    public static class RootPathsDemo
    {
        public static List<List<Vector2>> GetDemoPaths()
        {
            // Стартовая точка
            Vector2 start = new Vector2(0, 0);

            // Прямой путь вниз
            var path1 = new List<Vector2>
            {
                start,
                start + Vector2.down * 1,
                start + Vector2.down * 2,
                start + Vector2.down * 3
            };

            // Путь с поворотом влево
            var path2 = new List<Vector2>
            {
                start,
                start + Vector2.down * 1,
                start + Vector2.down * 2,
                start + Vector2.down * 2 + Vector2.left * 1,
                start + Vector2.down * 2 + Vector2.left * 2
            };

            // Путь с поворотом вправо
            var path3 = new List<Vector2>
            {
                start,
                start + Vector2.down * 1,
                start + Vector2.down * 2,
                start + Vector2.down * 2 + Vector2.right * 1,
                start + Vector2.down * 2 + Vector2.right * 2
            };

            // Короткое боковое ответвление
            var path4 = new List<Vector2>
            {
                start,
                start + Vector2.left * 1,
                start + Vector2.left * 2
            };

            // Диагональный путь
            var path5 = new List<Vector2>
            {
                start,
                start + new Vector2(-1, -1),
                start + new Vector2(-2, -2),
                start + new Vector2(-3, -3)
            };

            return new List<List<Vector2>> { path1, path2, path3, path4, path5 };
        }
    }
}
