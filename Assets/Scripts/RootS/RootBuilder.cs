using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBuilder : MonoBehaviour
{
    public PlantRoots PlantRoots = new PlantRoots();

    public void BuildRoot(int parentInd, List<Vector2> rootPositions)
    {
        //TODO: Creating rootNode objects, making graph, starting growing coroutine
        for(int i = 0; i < rootPositions.Count; i++)
        {
            PlantRoots.CreateNode(parentInd, rootPositions[i]);
        }
    }
}
