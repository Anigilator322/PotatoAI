using Assets.Scripts.RootS.Plants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "PrefabsReferenceMap", order = 0)]
public class Prefabs : ScriptableObject
{
    [SerializeField]
    public Plant plantPrefab;
}
