
using Assets.Scripts.RootS;
using Cysharp.Threading.Tasks;
using ModestTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
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
        private GrowingRoots _growingRoots;
        private RootSpawnSystem _rootSpawnSystem;
        private float _growthTickTime = 1f;

        private CancellationTokenSource _growRootsCancellationTokenSource = new CancellationTokenSource();
        //конфигурация скорости роста и т.п.

        public void StartGrowth(RootBlueprint blueprint)
        {
            _growingRoots.Blueprints.Add(blueprint.Id, new GrowingRoot(blueprint)
            {
                State = GrowthState.Growing
            });
            StartGrowingCoroutine();
        }

        private bool IsCoroutineRunning()
        {
            return _growRootsCancellationTokenSource != null;
        }

        private void StartGrowingCoroutine()
        {
            if(_growingRoots.Blueprints.Count > 0 && !IsCoroutineRunning())
            {
                _growRootsCancellationTokenSource = new CancellationTokenSource();
                UniTask.RunOnThreadPool(() => GrowRoots(_growRootsCancellationTokenSource.Token));
            }
        }

        private void StopGrowingCoroutine()
        {
            if(IsCoroutineRunning())
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
            while(_growingRoots.Blueprints.Count > 0 || cancellationToken.IsCancellationRequested)
            {
                foreach (var growingRoot in _growingRoots.Blueprints)
                {
                    switch (growingRoot.Value.State)
                    {
                        case GrowthState.Growing:
                            SpawnNode(growingRoot.Value);
                            break;
                        case GrowthState.Paused:

                            break;
                        case GrowthState.Canceled:
                            _growingRoots.RemoveBlueprint(growingRoot.Value);
                            break;
                        case GrowthState.Failed:
                            _growingRoots.RemoveBlueprint(growingRoot.Value);
                            break;
                        case GrowthState.Completed:
                            _growingRoots.RemoveBlueprint(growingRoot.Value);
                            break;
                    }
                    if (growingRoot.Value.Blueprint.RootPath.Count == 0)
                    {
                        growingRoot.Value.State = GrowthState.Completed;
                    }
                }
                await UniTask.Delay(TimeSpan.FromSeconds(_growthTickTime));
            }
            StopGrowingCoroutine();
        }

        private void SpawnNode(GrowingRoot growingRoot)
        {
            RootNode node = _rootSpawnSystem.TrySpawnRoot(growingRoot);
            growingRoot.Blueprint.RootPath.RemoveAt(0);
            growingRoot.Blueprint.RootNode = node;

            if (growingRoot.Blueprint.RootPath.Count == 0)
            {
                growingRoot.State = GrowthState.Completed;
            }
        }
    }

    internal class GrowingRoots
    {
        public Dictionary<string,GrowingRoot> Blueprints { get; private set; }

        public void RemoveBlueprint(GrowingRoot root)
        {
            Blueprints.Remove(root.Blueprint.Id);
        }
    }
}
