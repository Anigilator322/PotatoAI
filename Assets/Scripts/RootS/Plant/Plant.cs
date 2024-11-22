using Assets.Scripts.Map;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.RootS.Plants
{
    public class Plant : MonoBehaviour
    {
        [Inject] public PlantRoots Roots;

        private void Start()
        {
            Debug.Log(Roots.Nodes.Count);
        }
    }
}
