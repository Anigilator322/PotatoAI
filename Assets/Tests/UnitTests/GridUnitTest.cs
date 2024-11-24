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
        PlantRoots rootSystem = new PlantRoots();
        GridPartition<RootNode> grid = new GridPartition<RootNode>(1);
        Container.Bind<GridPartition<RootNode>>().FromInstance(grid).AsSingle();
        Container.Bind<PlantRoots>().FromInstance(rootSystem).AsSingle();
    }


    [Test]
    public void TestInitialValues()
    {
        var grid = Container.Resolve<GridPartition<RootNode>>();
        var rootSystem = Container.Resolve<PlantRoots>();
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(0.5f, 0.5f));
        points.Add(new Vector2(0.5f, 0));
        points.Add(new Vector2(0.6f, 0));
        points.Add(new Vector2(1, 0));
        points.Add(new Vector2(0.7f, 0.7f));
        points.Add(new Vector2(0.2f, -0.2f));
        points.Add(new Vector2(0.2f, 0));
        points.Add(new Vector2(1.5f, 0));
        foreach (var point in points)
        {
            RootNode node = new RootNode(point);
            rootSystem.Nodes.Add(node);
            grid.Insert(node);
        }
        List<RootNode> pointsIndexes = grid.Query(0.5f, new Vector2(0, 0));
        Assert.That(pointsIndexes.Count == 6);
    }

    [Test]
    public void TestInitialValues2()
    {
        var grid = Container.Resolve<GridPartition<RootNode>>();
        var rootSystem = Container.Resolve<PlantRoots>();
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(-0.5f, -0.5f));
        points.Add(new Vector2(-0.5f, 0));
        points.Add(new Vector2(-0.6f, 0));
        points.Add(new Vector2(-1, 0));
        points.Add(new Vector2(-0.7f, -0.7f));
        points.Add(new Vector2(-0.2f, 0.2f));
        points.Add(new Vector2(-0.2f, 0));
        points.Add(new Vector2(-1.5f, 0));
        foreach (var point in points)
        {
            RootNode node = new RootNode(point);
            rootSystem.Nodes.Add(node);
            grid.Insert(node);
        }
        
        List<RootNode> pointsIndexes = grid.Query(0.5f, new Vector2(0, 0));
        Assert.That(pointsIndexes.Count == 7);
    }
    [Test]
    public void TestInitValues3()
    {
        var grid = Container.Resolve<GridPartition<RootNode>>();
        var rootSystem = Container.Resolve<PlantRoots>();
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(0.5f, 0.5f));
        points.Add(new Vector2(93f, 42f));
        foreach (var point in points)
        {
            RootNode node = new RootNode(point);
            rootSystem.Nodes.Add(node);
            grid.Insert(node);
        }

        List<RootNode> pointsIndexes = grid.Query(100, new Vector2(0, 0));
        Assert.That(pointsIndexes.Count == 1);
    }
}