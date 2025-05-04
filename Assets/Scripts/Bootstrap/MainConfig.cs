using Assets.Scripts.Roots.Plants;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Bootstrap
{
    [CreateAssetMenu(fileName = "MainConfig", menuName = "GameConfigs/Main", order = 2)]
    public class MainConfig : ScriptableObject
    {
        [SerializeField]
        public int gameDurationSeconds = 90;

        [SerializeField]
        public int startCalories = 500;

        [SerializeField]
        public float growthTickTime = 0.1f;

        [SerializeField]
        public float clickNodeSearchRadius = 2;

        [SerializeField]
        public float rootSegmentLength = 0.8f;

        [SerializeField]
        public float maxBuildAngle = 15f;

        [SerializeField]
        public float rootCostGrowthExponent = 1.25f,
            reconCost = 1.5f,
            wallCost = 2f;
    }
}
