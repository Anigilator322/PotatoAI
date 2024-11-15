using Zenject;
using NUnit.Framework;
using UnityEngine;
using Assets.Scripts.Map;
using System.Collections.Generic;
using Assets.Scripts.RootS;

[TestFixture]
public class GridUnitTest : ZenjectUnitTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        GridPartition<RootNode> grid = new GridPartition<RootNode>(2);
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(0.5f, 0.5f));
        points.Add(new Vector2(0.5f, 0));
        points.Add(new Vector2(0.6f, 0));
        points.Add(new Vector2(1, 0));
        points.Add(new Vector2(0.7f, 0.7f));
        points.Add(new Vector2(0.2f, -0.2f));
        points.Add(new Vector2(0.2f, 0));
        points.Add(new Vector2(1.5f, 0));
        for (int i = 0; i < 8; i++)
        {
            grid.Insert(points[i], i);
        }
        Container.Bind<GridPartition<RootNode>>().FromInstance(grid).AsSingle();
    }

    [Test]
    public void TestInitialValues()
    {
        var grid = Container.Resolve<GridPartition<RootNode>>();
        List<int> pointsIndexes = grid.Query(0.5f, new Vector2(0, 0));
        Dictionary<Vector2Int, Cell<RootNode>> cells = grid.GetAllCellsWithPoints();
        Debug.Log("cells count: " + cells.Count);
        foreach (var cell in cells)
        {
            Debug.Log("---------------");
            Debug.Log("Cell LeftBottomCorner: " + cell.Key);
            
            Cell<RootNode> cellc = cell.Value;
            foreach (var indexes in cellc.GetIndexes())
                Debug.Log("indexes: " + indexes);
            Debug.Log("---------------");
        }
        Debug.Log("---------All indexes------------");
        Debug.Log("All returned indexes count: " +pointsIndexes.Count);
        foreach(var index in pointsIndexes)
        {
            Debug.Log(index);
        }
        Assert.That(pointsIndexes.Count == 6);
    }

}