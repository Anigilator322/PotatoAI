using Zenject;
using NUnit.Framework;
using UnityEngine;
using Assets.Scripts.Map;
using Codice.Client.Common.TreeGrouper;

[TestFixture]
public class GridUnitTest : ZenjectUnitTestFixture
{
    [SetUp]
    public void CommonInstall()
    {
        GridPartition<Node> grid = new GridPartition<Node>(1);
        Container.Bind<GridPartition<Node>>().FromInstance(grid).AsSingle();
    }

    [Test]
    public void TestInitialValues()
    {
        var logger = Container.Resolve<GridPartition<Node>>();
        Debug.Log(logger.GetAllCellsWithPoints().Count);
        Assert.That(logger.GetAllCellsWithPoints().Count == 0);
    }

}