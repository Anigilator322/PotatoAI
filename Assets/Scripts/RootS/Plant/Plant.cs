using Assets.Scripts.Map;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.RootS.Plants
{
    public class Plant : MonoBehaviour
    {
        public PlantRoots Roots;

        [Inject]
        private void Construct(PlantRoots root)
        {
            Roots = root;
        }

        private void Start()
        {
            Debug.Log(Roots.Nodes.Count);
        }
    }
}
