using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilityCapsule
    {
        public VisibilityCapsule(Vector3 position1, Vector3 position2, float revealRadius)
        {
            this.Start = position1;
            this.End = position2;
            this.Radius = revealRadius;
        }

        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Radius { get; set; }
    }
}
