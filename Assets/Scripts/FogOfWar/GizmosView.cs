using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.FogOfWar
{
    public class GizmosView : MonoBehaviour
    {
        [Inject] private FieldOfView fov; 
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach(var obj in fov._visiblePoints)
            {
                Gizmos.DrawSphere(obj.Transform.position, 0.2f);
            }

            foreach(var obs in fov.BlockingPoints)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(obs.Transform.position, 0.1f);
            }
        }
    }
}