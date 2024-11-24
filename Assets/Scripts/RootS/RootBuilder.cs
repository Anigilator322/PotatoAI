using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using Assets.Scripts.RootS.Plants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RootBuilder
{
    private Plant _plant;
    private RootView _rootView;
    private GridPartition<RootNode> _gridPartition;

    public void BuildRoot(RootNode parent, List<Vector2> rootPositions)
    {
        for(int i = 0; i < rootPositions.Count; i++)
        {
            CreateNode(parent, rootPositions[i]);
        }
    }

    private void CreateNode(RootNode parent, Vector2 pos)
    {
        RootNode node = new RootNode(pos, parent);
        parent.SetChildren(node);
        _gridPartition.Insert(node);
    }


}
