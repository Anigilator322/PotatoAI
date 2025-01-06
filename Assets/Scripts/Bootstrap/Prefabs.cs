using Assets.Scripts.Roots.Plants;
using UnityEngine;
namespace Assets.Scripts.Bootstrap
{
    [CreateAssetMenu(fileName = "Prefabs", menuName = "PrefabsReferenceMap", order = 0)]
    public class Prefabs : ScriptableObject
    {
        [SerializeField]
        public Plant plantPrefab;

        [SerializeField]
        public SpriteRenderer soilPrefab;
    }
}
