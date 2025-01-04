
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;

namespace Assets.Scripts
{
    public class GrowingRoot
    {
        /// <summary>
        /// A plant this root belongs to
        /// </summary>
        public Plant Plant { get; }

        public GrowthState State { get; set; }

        public RootBlueprint Blueprint { get; set; }

        public GrowingRoot(RootBlueprint blueprint, Plant plant)
        {
            Blueprint = blueprint;
            Plant = plant;
        }
    }
}
