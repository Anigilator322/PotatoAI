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
            Gizmos.color = Color.red;
            foreach(var obj in fov._visiblePoints)
            {
                Gizmos.DrawSphere(obj.Position, 0.1f);
            }
        }
    }
}