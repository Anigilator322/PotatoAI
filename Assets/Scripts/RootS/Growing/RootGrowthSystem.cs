
using Assets.Scripts.RootS;
using Cysharp.Threading.Tasks;
using ModestTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private List<UniTask> _activeGrowingTasks = new List<UniTask>();
        //конфигурация скорости роста и т.п.

        public void StartGrowth(RootBlueprint blueprint)
        {
            _growingRoots.Blueprints.Add(blueprint.Id, new GrowingRoot(blueprint)
            {
                State = GrowthState.Growing
            });
            var task = GrowRoot(_growingRoots.Blueprints[blueprint.Id]);
            _activeGrowingTasks.Add(task);
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

        private async UniTask GrowRoot(GrowingRoot growingRoot)
        {
            while(growingRoot.RemainingNodes.Count > 0)
            {
                switch (growingRoot.State)
                {
                    case GrowthState.Growing:
                        SpawnNode(growingRoot);
                        await UniTask.Delay(TimeSpan.FromSeconds(_growthTickTime));
                        break;
                    case GrowthState.Paused:
                        await UniTask.WaitWhile(() => growingRoot.State == GrowthState.Paused);
                        break;
                    case GrowthState.Canceled:
                        CancelGrowth(growingRoot);
                        return;
                    case GrowthState.Failed:
                        CancelGrowth(growingRoot);
                        return;
                    case GrowthState.Completed:
                        CancelGrowth(growingRoot);
                        return;
                }
            }
            growingRoot.State = GrowthState.Completed;
            CancelGrowth(growingRoot);
        }

        private void SpawnNode(GrowingRoot growingRoot)
        {
            
            if(!_rootSpawnSystem.TrySpawnRoot(growingRoot.ParentOfFirstInQueue,growingRoot.RemainingNodes.Dequeue(), growingRoot.Blueprint.RootType));
                growingRoot.State = GrowthState.Failed;
            //создание узла
        }

        private void CancelGrowth(GrowingRoot root)
        {
            _growingRoots.Blueprints.Remove(root.Blueprint.Id);
        }
    }

    internal class GrowingRoot
    {
        public GrowthState State { get; set; }

        public RootBlueprint Blueprint { get; set; }
        public Queue<Vector2> RemainingNodes { get; private set; }
        public RootNode ParentOfFirstInQueue { get; set; }

        public GrowingRoot(RootBlueprint blueprint)
        {
            Blueprint = blueprint;
            ParentOfFirstInQueue = blueprint.RootNode;
            RemainingNodes = new Queue<Vector2>(Blueprint.RootPath);
        }
    }

    internal class GrowingRoots
    {
        public Dictionary<string,GrowingRoot> Blueprints { get; private set; }
    }
}
