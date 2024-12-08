using Assets.Scripts.RootS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        GrowingRoots _growingRoots;

        //конфигурация скорости роста и т.п.

        public void StartGrowth(RootBlueprint blueprint)
        {
            _growingRoots.Blueprints.Add(blueprint.Id, new GrowingRoot()
            {
                State = GrowthState.Growing,
                Blueprint = blueprint
            });
        }

        public GrowthState GetGrowingRootState(string id)
        {
            return _growingRoots.Blueprints[id]
                .State;
        }

        public bool CancelGrowth(string id)
        {
            GrowingRoot root = _growingRoots.Blueprints[id];
            if (root is null)
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
    }

    internal class GrowingRoot
    {
        public GrowthState State { get; set; }

        public RootBlueprint Blueprint { get; set; }
    }

    internal class GrowingRoots
    {
        public Dictionary<string, GrowingRoot> Blueprints { get; private set; }
    }
}