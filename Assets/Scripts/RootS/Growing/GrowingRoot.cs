
using Assets.Scripts.RootS;

namespace Assets.Scripts
{
    public class GrowingRoot
    {
        public GrowthState State { get; set; }

        public RootBlueprint Blueprint { get; set; }

        public GrowingRoot(RootBlueprint blueprint)
        {
            Blueprint = blueprint;
        }
    }
}
