using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.Plants
{
    public class PlantsModel
    {
        public List<Plant> Plants { get; set; } = new List<Plant> { };

        public void Reset()
        {
            foreach (var plant in Plants) {
                GameObject.Destroy(plant.gameObject);
            }
            Plants = new();
        }
    }
}
