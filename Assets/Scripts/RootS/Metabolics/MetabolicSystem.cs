using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RootS.Metabolics
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