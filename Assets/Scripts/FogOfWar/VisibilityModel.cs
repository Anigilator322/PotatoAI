using Assets.Scripts.Map;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.UX;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilityModel
    {
        #region VisibilitySystem
        public List<VisibilityCapsule> VisibilityCapsules;
        public Dictionary<Plant, List<IPositionedObject>> VisibleByPlantsPoints;
        #endregion
        
        public VisibilityModel()
        {
            VisibilityCapsules = new List<VisibilityCapsule>();
            VisibleByPlantsPoints = new Dictionary<Plant, List<IPositionedObject>>();
        }

        public void Reset()
        {
            VisibilityCapsules.Clear();
            VisibleByPlantsPoints.Clear();
        }
    }
}
