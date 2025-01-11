using UnityEngine;
using Assets.Scripts.Roots.Plants;

namespace Assets.Scripts.Roots.Tools
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
                    Gizmos.DrawSphere(node.Transform.position, 0.1f);
                }
            }
        }
    }
}
