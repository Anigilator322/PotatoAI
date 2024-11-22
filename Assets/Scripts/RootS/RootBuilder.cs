using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBuilder : MonoBehaviour
{
    [SerializeField] private Plant _plant;
    [SerializeField] private RootView _rootView;
    public void BuildRoot(int parentInd, List<Vector2> rootPositions)
    {
        for(int i = 0; i < rootPositions.Count; i++)
        {
            _plant.Roots.CreateNode(parentInd, rootPositions[i]);
        }
    }

    
}
