namespace Assets.Scripts.Roots.RootsBuilding.Growing
{
    interface IRootGrowthSystem
    {
        void StartGrowth(RootBlueprint rootPath);

        RootBlueprint GetBlueprint(string id);

        bool CancelGrowth(string id);
    }
}
