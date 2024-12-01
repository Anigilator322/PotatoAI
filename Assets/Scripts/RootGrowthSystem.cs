
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


    interface IRootGrowthSystem
    {
        void StartGrowth(RootBuildingPath rootPath);
        bool CancelGrowth(string id);
    }

    public class RootGrowthSystem : IRootGrowthSystem
    {
        GrowingRoots _growingRoots;

        //конфигурация скорости роста и т.п.

        public void StartGrowth(RootBuildingPath rootPath)
        {
            _growingRoots.Roots.Add(new GrowingRoot()
            {
                State = GrowthState.Growing,
                Path = rootPath
            });
        }

        public GrowthState GetGrowingRootState(string id)
        {
            return _growingRoots.Roots
                .Single(x => x.Path.Id == id)
                .State;
        }

        public bool CancelGrowth(string id)
        {
            GrowingRoot root = _growingRoots.Roots.SingleOrDefault(x => x.Path.Id == id);
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
    }

    public class GrowingRoot : RootBuildingPath
    {
        public GrowthState State { get; set; }

        public RootBuildingPath Path { get; set; }

        public int BuildingNodeIndex { get; set; } = 0; 
    }

    public class GrowingRoots
    {
        // Id, Сам отросток
        public List<GrowingRoot> Roots { get; private set; }
    }
}
