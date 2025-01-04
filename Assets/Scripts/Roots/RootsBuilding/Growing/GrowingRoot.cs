using Assets.Scripts.Roots.Plants;

namespace Assets.Scripts.Roots.RootsBuilding.Growing
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
