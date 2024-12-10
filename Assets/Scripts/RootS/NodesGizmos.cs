using UnityEngine;
using Assets.Scripts.RootS.Plants;

namespace Assets.Scripts.RootS
{
    public class NodesGizmos : MonoBehaviour
    {
        [SerializeField] Plant plant;

        private void OnDrawGizmos()
        {
            if (plant.Roots.Nodes.Count > 0)
            {
                foreach (var node in plant.Roots.Nodes)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(node.Position, 0.1f);
                }
            }
        }
    }
}
