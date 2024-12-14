using Assets.Scripts.Map;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.RootS.Plants
{
    public class Plant : MonoBehaviour
    {
        [Inject]
        public PlantRoots Roots;
        [Inject]
        GridPartition<RootNode> GridPartition; // This field used for testing purposes

        private void Start()
        {
            //Roots.Nodes.Add(new RootNode(gameObject.transform.position));
            //GridPartition.Insert(Roots.Nodes[0]);
        }
    }
}
