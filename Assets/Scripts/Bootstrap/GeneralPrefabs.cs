using Assets.Scripts.Roots.Plants;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Bootstrap
{
    [CreateAssetMenu(fileName = "Prefabs_general", menuName = "GameConfigs/PrefabsCommonMap", order = 0)]
    public class GeneralPrefabs : ScriptableObject
    {
        [SerializeField]
        public Plant plantPrefab;

        [SerializeField]
        public Soil soilPrefab;
    }
}
