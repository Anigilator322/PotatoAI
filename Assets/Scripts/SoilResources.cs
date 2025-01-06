using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilResources
{
    const int CELL_SIZE = 1;
    public List<ResourcePoint> Resources { get; set; } = new List<ResourcePoint>();

    private readonly GridPartition<ResourcePoint> _gridPartition  = new GridPartition<ResourcePoint>(CELL_SIZE);

    public void AddResource(ResourcePoint resourcePoint)
    {
        Resources.Add(resourcePoint);
        _gridPartition.Insert(resourcePoint);
    }

    public List<ResourcePoint> GetResourcesFromCircle(float circleRadius, Vector2 circleCenter)
    {
        return _gridPartition.Query(circleRadius, circleCenter);
    }
}
