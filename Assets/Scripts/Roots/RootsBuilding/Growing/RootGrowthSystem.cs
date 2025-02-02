using Assets.Scripts.Roots.Plants;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding.Growing
{
    public enum GrowthState
    {
        Paused = 0,
        Growing = 1,
        Completed = 2,
        Failed = 3,
        Canceled = 4,
    }

    public class RootGrowthSystem : IRootGrowthSystem
    {
        private GrowingRoots _growingRoots = new GrowingRoots();
        private RootSpawnSystem _rootSpawnSystem;
        private SynchronizationContext _mainThreadContext;

        private PlantsModel PlantsModel { get; }
        private float _growthTickTime = 0.1f;

        private CancellationTokenSource _growRootsCancellationTokenSource;

        public RootGrowthSystem(RootSpawnSystem rootSpawnSystem, PlantsModel plantsModel)
        {
            PlantsModel = plantsModel;
            _rootSpawnSystem = rootSpawnSystem;
            _mainThreadContext = System.Threading.SynchronizationContext.Current;
        }

        public void StartGrowth(RootBlueprint blueprint)
        {
            Plant plant = PlantsModel.Plants
                .SingleOrDefault(r => r.Roots.Nodes.Contains(blueprint.StartRootNode))
                ?? throw new Exception($"No {nameof(Plant)} for {nameof(RootBlueprint)} trying to grow found");

            _growingRoots.Blueprints.Add(blueprint.Id, new GrowingRoot(blueprint, plant)
            {
                State = GrowthState.Growing
            });

            Debug.Log("Blueprint added to growing roots, rootPath count: " + blueprint.RootPath.Count);
            StartGrowingCoroutine();
        }

        bool coroutineIsRunning = false;

        private bool IsCoroutineRunning()
        {
            return _growRootsCancellationTokenSource is not null;
        }

        private void StartGrowingCoroutine()
        {
            if(_growingRoots.Blueprints.Count > 0 && !IsCoroutineRunning())
            {
                Debug.Log("Starting coroutine");
                _growRootsCancellationTokenSource = new CancellationTokenSource();
                UniTask.RunOnThreadPool(() => GrowRoots(_growRootsCancellationTokenSource.Token));
            }
        }

        private void StopGrowingCoroutine()
        {
            Debug.Log("Stopping coroutine");
            Debug.Log("Is coroutine Running?: "+ IsCoroutineRunning());
            if (IsCoroutineRunning())
            {
                _growRootsCancellationTokenSource.Cancel();
                _growRootsCancellationTokenSource.Dispose();
                _growRootsCancellationTokenSource = null;
            }
        }

        public GrowthState GetGrowingRootState(string id)
        {
            return _growingRoots.Blueprints[id]
                .State;
        }

        public bool CancelGrowth(string id)
        {
            GrowingRoot root = _growingRoots.Blueprints[id];
            if(root is null)
            {
                return false;
            }
            else
            {
                root.State = GrowthState.Canceled;
                return true;
            }
        }

        public RootBlueprint GetBlueprint(string id)
        {
            RootBlueprint blueprint = _growingRoots.Blueprints[id].Blueprint;
            if (blueprint is null)
                throw new Exception("Trying to get rootBlueprint with wrong ID");
            else
                return blueprint;
        }

        private async UniTask GrowRoots(CancellationToken cancellationToken)
        {
            Debug.Log("Start growing roots");
            while (_growingRoots.Blueprints.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var ids = _growingRoots.Blueprints.Keys.ToArray();
                for (int i = 0; i < ids.Length; i++ )
                {
                    var id = ids[i];
                    var growingRoot = _growingRoots.Blueprints[ids[i]];
                    //Debug.Log("Growing root " + id);

                    switch (growingRoot.State)
                    {
                        case GrowthState.Growing:

                            //Debug.Log("Spawn for root " + id);
                            _mainThreadContext.Post(_ => SpawnNode(growingRoot), null);
                            break;
                        
                        case GrowthState.Paused:
                            break;

                        case GrowthState.Canceled:
                        case GrowthState.Failed:
                        case GrowthState.Completed:
                            //Debug.Log("Remove root " + id);
                            _growingRoots.RemoveBlueprint(id);

                            break;
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_growthTickTime));
            }
            StopGrowingCoroutine();
        }

        private void SpawnNode(GrowingRoot growingRoot)
        {

            RootNode parent = growingRoot.Blueprint.StartRootNode;
            Vector2 position = growingRoot.Blueprint.RootPath[0];
            RootType type = growingRoot.Blueprint.RootType;

            RootNode node = _rootSpawnSystem.SpawnRootNode(
                new RootNode(position, parent, type));

            growingRoot.Blueprint.RemoveFirstPoint();
            growingRoot.Blueprint.StartRootNode = node;

            if (growingRoot.Blueprint.RootPath.Count == 0)
            {
                growingRoot.State = GrowthState.Completed;
            }
        }
    }

    internal class GrowingRoots
    {
        public Dictionary<string, GrowingRoot> Blueprints { get; private set; } = new Dictionary<string, GrowingRoot>();

        public void RemoveBlueprint(GrowingRoot root)
        {
            RemoveBlueprint(root.Blueprint.Id);
        }

        public void RemoveBlueprint(string id)
        {
            Blueprints.Remove(id);
        }
    }
}
