using Assets.Scripts.Roots.RootsBuilding;
using System.Collections.Generic;

namespace Assets.Scripts.Roots.Metabolics
{
    public class MetabolicSystem
    {
        public int Callories { get; private set; }
        public Dictionary<string, int> ReservedCallories { get; private set; }

        public bool IsAbleToBuild(RootBlueprint blueprint)
        {
            int price = CalculateBlueprintPrice(blueprint);
            if (price <= Callories)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int CalculateBlueprintPrice(RootBlueprint blueprint)
        {
            return 0;
        }

    }
}