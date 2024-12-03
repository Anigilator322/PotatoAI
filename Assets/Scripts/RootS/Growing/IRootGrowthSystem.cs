
using Assets.Scripts.RootS;

namespace Assets.Scripts
{
    interface IRootGrowthSystem
    {
        void StartGrowth(RootBlueprint rootPath);

        RootBlueprint GetBlueprint(string id);

        bool CancelGrowth(string id);
    }
}
