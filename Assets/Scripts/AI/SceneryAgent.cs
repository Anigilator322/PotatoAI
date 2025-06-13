using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.Roots.RootsBuilding.Growing;
using System.Collections.Generic;
using System.Linq;
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

                var firstNodes = _plantsModel.Plants[0].Roots.GetNodesFromCircle(1f, path[0]);
                baseRoot = firstNodes.Any() ? firstNodes[0] : baseRoot;
                _mainThreadContext.Send(_ => drawingRoot = DrawingRootBlueprint.Create(RootType.Harvester, baseRoot), null);
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
            Vector2 start = new Vector2(0, 0);

            // Основной ствол вниз
            var mainRoot = new List<Vector2>
            {
                start,
                start + Vector2.down * 1,
                start + Vector2.down * 2,
                start + Vector2.down * 3,
                start + Vector2.down * 4,
                start + Vector2.down * 5,
                start + Vector2.down * 6,
                start + Vector2.down * 7,
                start + Vector2.down * 8,
                start + Vector2.down * 9,
                start + Vector2.down * 10
            };

            // Ветки от основного ствола
            var leftBranch1 = new List<Vector2>
            {
                start + Vector2.down * 2,
                start + Vector2.down * 2 + Vector2.left * 1,
                start + Vector2.down * 2 + Vector2.left * 2,
                start + Vector2.down * 2 + Vector2.left * 3,
                start + Vector2.down * 2 + Vector2.left * 4
            };

            var rightBranch1 = new List<Vector2>
            {
                start + Vector2.down * 3,
                start + Vector2.down * 3 + Vector2.right * 1,
                start + Vector2.down * 3 + Vector2.right * 2,
                start + Vector2.down * 3 + Vector2.right * 3,
                start + Vector2.down * 3 + Vector2.right * 4
            };

            var leftBranch2 = new List<Vector2>
            {
                start + Vector2.down * 6,
                start + Vector2.down * 6 + Vector2.left * 1,
                start + Vector2.down * 6 + Vector2.left * 2,
                start + Vector2.down * 6 + Vector2.left * 3,
                start + Vector2.down * 6 + Vector2.left * 4,
                start + Vector2.down * 6 + Vector2.left * 5
            };

            var rightBranch2 = new List<Vector2>
            {
                start + Vector2.down * 8,
                start + Vector2.down * 8 + Vector2.right * 1,
                start + Vector2.down * 8 + Vector2.right * 2,
                start + Vector2.down * 8 + Vector2.right * 3,
                start + Vector2.down * 8 + Vector2.right * 4,
                start + Vector2.down * 8 + Vector2.right * 5
            };

            // Диагональные длинные ветки
            var diagLeftLong = new List<Vector2>
            {
                start + Vector2.down * 5,
                start + new Vector2(-1, -6),
                start + new Vector2(-2, -7),
                start + new Vector2(-3, -8),
                start + new Vector2(-4, -9),
                start + new Vector2(-5, -10)
            };

            var diagRightLong = new List<Vector2>
            {
                start + Vector2.down * 7,
                start + new Vector2(1, -8),
                start + new Vector2(2, -9),
                start + new Vector2(3, -10),
                start + new Vector2(4, -11),
                start + new Vector2(5, -12)
            };

            // Короткие боковые ветки у основания
            var shortLeft = new List<Vector2>
            {
                start,
                start + Vector2.left * 1,
                start + Vector2.left * 2,
                start + Vector2.left * 3
            };

            var shortRight = new List<Vector2>
            {
                start,
                start + Vector2.right * 1,
                start + Vector2.right * 2,
                start + Vector2.right * 3
            };

            // Ветка вверх (имитация случайного роста)
            var upBranch = new List<Vector2>
            {
                start,
                start + Vector2.up * 1,
                start + Vector2.up * 2,
                start + Vector2.up * 3
            };

            // Ветка-змейка влево-вниз
            var snakeLeft = new List<Vector2>
            {
                start + Vector2.down * 4,
                start + Vector2.down * 5 + Vector2.left * 1,
                start + Vector2.down * 6,
                start + Vector2.down * 7 + Vector2.left * 1,
                start + Vector2.down * 8,
                start + Vector2.down * 9 + Vector2.left * 1
            };

            // Ветка-змейка вправо-вниз
            var snakeRight = new List<Vector2>
            {
                start + Vector2.down * 4,
                start + Vector2.down * 5 + Vector2.right * 1,
                start + Vector2.down * 6,
                start + Vector2.down * 7 + Vector2.right * 1,
                start + Vector2.down * 8,
                start + Vector2.down * 9 + Vector2.right * 1
            };

            // Ветка из конца левой ветки вниз
            Vector2 leftBranchEnd = start + Vector2.down * 2 + Vector2.left * 4;
            var leftDown = new List<Vector2>
            {
                leftBranchEnd,
                leftBranchEnd + Vector2.down * 1,
                leftBranchEnd + Vector2.down * 2,
                leftBranchEnd + Vector2.down * 3
            };

            // Ветка из конца правой ветки вниз
            Vector2 rightBranchEnd = start + Vector2.down * 3 + Vector2.right * 4;
            var rightDown = new List<Vector2>
            {
                rightBranchEnd,
                rightBranchEnd + Vector2.down * 1,
                rightBranchEnd + Vector2.down * 2,
                rightBranchEnd + Vector2.down * 3
            };

            // Ветка из конца диагональной левой ветки влево
            Vector2 diagLeftEnd = start + new Vector2(-5, -10);
            var diagLeftSide = new List<Vector2>
            {
                diagLeftEnd,
                diagLeftEnd + Vector2.left * 1,
                diagLeftEnd + Vector2.left * 2
            };

            // Ветка из конца диагональной правой ветки вправо
            Vector2 diagRightEnd = start + new Vector2(5, -12);
            var diagRightSide = new List<Vector2>
            {
                diagRightEnd,
                diagRightEnd + Vector2.right * 1,
                diagRightEnd + Vector2.right * 2
            };

            // Короткая диагональная ветка вправо-вверх
            var diagUpRight = new List<Vector2>
            {
                start,
                start + new Vector2(1, 1),
                start + new Vector2(2, 2)
            };

            // Короткая диагональная ветка влево-вверх
            var diagUpLeft = new List<Vector2>
            {
                start,
                start + new Vector2(-1, 1),
                start + new Vector2(-2, 2)
            };

            // Ветка из середины основного ствола влево-вниз
            var midLeft = new List<Vector2>
            {
                start + Vector2.down * 5,
                start + Vector2.down * 6 + Vector2.left * 1,
                start + Vector2.down * 7 + Vector2.left * 2,
                start + Vector2.down * 8 + Vector2.left * 3
            };

            // Ветка из середины основного ствола вправо-вниз
            var midRight = new List<Vector2>
            {
                start + Vector2.down * 5,
                start + Vector2.down * 6 + Vector2.right * 1,
                start + Vector2.down * 7 + Vector2.right * 2,
                start + Vector2.down * 8 + Vector2.right * 3
            };

            // Очень длинная диагональная ветка влево-вниз
            var longDiagLeft = new List<Vector2>
            {
                start,
                start + new Vector2(-1, -1),
                start + new Vector2(-2, -2),
                start + new Vector2(-3, -3),
                start + new Vector2(-4, -4),
                start + new Vector2(-5, -5),
                start + new Vector2(-6, -6),
                start + new Vector2(-7, -7),
                start + new Vector2(-8, -8),
                start + new Vector2(-9, -9),
                start + new Vector2(-10, -10)
            };

            // Очень длинная диагональная ветка вправо-вниз
            var longDiagRight = new List<Vector2>
            {
                start,
                start + new Vector2(1, -1),
                start + new Vector2(2, -2),
                start + new Vector2(3, -3),
                start + new Vector2(4, -4),
                start + new Vector2(5, -5),
                start + new Vector2(6, -6),
                start + new Vector2(7, -7),
                start + new Vector2(8, -8),
                start + new Vector2(9, -9),
                start + new Vector2(10, -10)
            };

            return new List<List<Vector2>>
            {
                mainRoot,
                leftBranch1,
                rightBranch1,
                leftBranch2,
                rightBranch2,
                diagLeftLong,
                diagRightLong,
                shortLeft,
                shortRight,
                upBranch,
                snakeLeft,
                snakeRight,
                leftDown,
                rightDown,
                diagLeftSide,
                diagRightSide,
                diagUpRight,
                diagUpLeft,
                midLeft,
                midRight,
                longDiagLeft,
                longDiagRight
            };
        }
    }
}
