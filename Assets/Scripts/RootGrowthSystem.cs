
using Assets.Scripts.RootS;
using System;
using System.Collections.Generic;

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


    interface IRootGrowthSystem
    {
        void StartGrowth(RootBuildingPath rootPath);
        void CancelGrowth(string id);
    }

    public class RootGrowthSystem : IRootGrowthSystem
    {
        GrowingRoots _growingRoots;

        //конфигурация скорости роста и т.п.

        public void StartGrowth(string id, RootNode rootNode)
        {
            _growingRoots.Roots[id] = new GrowingRoot(rootNode)
            {
                State = GrowthState.Growing
            };
        }

        public void CancelGrowth(string id)
        {
            _growingRoots.Roots[id].State = GrowthState.Canceled;
        }
    }

    public class GrowingRoot
    {
        public GrowingRoot(RootNode rootNodes)
        {
            Nodes = rootNodes;
        }

        public GrowthState State { get; set; }
        // Id, Сам отросток
        public RootNode Nodes { get; private set; }
    }

    public class GrowingRoots
    {
        // Id, Сам отросток
        public Dictionary<string, GrowingRoot> Roots { get; private set; }
    }
}
