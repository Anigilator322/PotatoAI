using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots;
using Assets.Scripts;
using Zenject;

namespace Assets.Scripts.Roots.View
{
    public class MeshCache : IInitializable
    {
        public Dictionary<Plant, Dictionary<RootType, MeshFilter>> meshFilters { get; private set; }

        private readonly Soil soil;
        private readonly PlantsModel plantsModel;

        public MeshCache(Soil soil, PlantsModel plantsModel)
        {
            this.soil = soil;
            this.plantsModel = plantsModel;
        }

        void IInitializable.Initialize()
        {
            Reset();
        }

        public void Reset()
        {
            meshFilters = new Dictionary<Plant, Dictionary<RootType, MeshFilter>>();

            var soilScale = soil.transform.localScale;

            foreach (Plant plant in plantsModel.Plants)
            {
                // Scale children
                for (int i = 0; i < plant.transform.childCount; i++)
                {
                    var child = plant.transform.GetChild(i);
                    if (child.name == "Roots" || child.name == "RootBlueprints")
                    {
                        child.localScale = new Vector3(1 / soilScale.x, 1 / soilScale.y, 1 / soilScale.z);
                    }
                }

                // Collect MeshFilters with RootTypeComponent
                foreach (MeshFilter meshFilter in plant.transform.GetComponentsInChildren<MeshFilter>())
                {
                    if (meshFilter.TryGetComponent(out RootTypeComponent rtc))
                    {
                        if (!meshFilters.TryGetValue(plant, out var dict))
                        {
                            dict = new Dictionary<RootType, MeshFilter>();
                            meshFilters[plant] = dict;
                        }

                        dict[rtc.rootType] = meshFilter;
                    }
                }
            }
        }

        public MeshFilter GetMeshFilter(Plant plant, RootType rootType)
        {
            return meshFilters.TryGetValue(plant, out var dict) && dict.TryGetValue(rootType, out var mf)
                ? mf
                : null;
        }
    }
}