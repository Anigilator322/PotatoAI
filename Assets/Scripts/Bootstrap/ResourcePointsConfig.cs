using Assets.Scripts.Roots.Plants;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Bootstrap
{
    [CreateAssetMenu(fileName = "ResourcePointsConfig", menuName = "GameConfigs/ResourcePoints", order = 1)]
    public class ResourcePointsConfig : ScriptableObject
    {
        [SerializeField]
        public Color water,
            phosphorus,
            potassium,
            nitrogen;

        [Tooltip("The size of any resource point")]
        [SerializeField]
        public float size;

        [SerializeField]
        public float maximumResourcesInPoint;

        [SerializeField]
        public int numberOfResourcePoints;
    }
}
