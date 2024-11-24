
using Assets.Scripts.RootS;
using System.Collections.Generic;

namespace Assets.Scripts
{
    interface IRootGrowthSystem
    {
        void StartGrowth(string id, RootNode rootNode);
        void CancelGrowth(string id);
    }

    public class RootGrowthSystem : IRootGrowthSystem
    {
        GrowingRoots _growingRoots;
        
        //конфигурация скорости роста и т.п.

        public void StartGrowth(string id, RootNode rootNode)
        {   
            _growingRoots.roots[id] = (rootNode, true);
        }

        public bool IsGrowing(string id)
        {
            return _growingRoots.roots[id].Item2;
        }

        public void CancelGrowth(string id)
        {
            _growingRoots.roots.Remove(id);
        }
    }

    public class GrowingRoots
    {
        // Id, Сам отросток
        public Dictionary<string, (RootNode, bool)> roots { get; private set; }
    }
}
