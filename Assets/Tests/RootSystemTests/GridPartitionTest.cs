using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GridPartitionTest
{
    GridPartition<RootNode> grid;
    List<RootNode> rootNodes;

    [SetUp]
    public void SetUp()
    {
        grid = new GridPartition<RootNode>(1);
        rootNodes = new List<RootNode>();

        rootNodes.Add(new RootNode(new Vector3(0,0)));
        rootNodes.Add(new RootNode(new Vector3(1, 0)));
        rootNodes.Add(new RootNode(new Vector3(0, 1)));
        rootNodes.Add(new RootNode(new Vector3(0.2f, 0.1f)));
        rootNodes.Add(new RootNode(new Vector3(-0.1f, 0.1f)));

        for(int i =0;i<rootNodes.Count;i++)
        {
            grid.Insert(rootNodes[i].Position, i);
        }

    }

    [UnityTest]
    public IEnumerator GridQuery()
    {
        List<int> indexes = new List<int>();
        indexes = grid.Query(1f, new Vector2(0, 0));

        yield return null;

        bool isTruePoint = true;
        if(!indexes.Contains(0))
        {
            isTruePoint = false;
        }
        if (!indexes.Contains(1))
        {
            isTruePoint = false;
        }
        if (!indexes.Contains(2))
        {
            isTruePoint = false;
        }
        if (!indexes.Contains(3))
        {
            isTruePoint = false;
        }
        if (!indexes.Contains(4))
        {
            isTruePoint = false;
        }

        Assert.IsTrue(isTruePoint);
    }
}
