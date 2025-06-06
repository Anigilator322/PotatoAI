using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Soil : MonoBehaviour
{
    public List<ResourcePoint> Resources { get; private set; } = new List<ResourcePoint>();

    // Not ideal concept separation !!!
    [SerializeField]
    public SpriteRenderer Sprite;

    const int CELL_SIZE = 1;
    private readonly GridPartition<ResourcePoint> _gridPartition  = new GridPartition<ResourcePoint>(CELL_SIZE);

    public void AddResource(ResourcePoint resourcePoint)
    {
        resourcePoint.Transform.parent = transform;
        Resources.Add(resourcePoint);
        _gridPartition.Insert(resourcePoint);
    }

    public void RemoveResource(ResourcePoint resourcePoint)
    {
        resourcePoint.Dismiss();
        Destroy(resourcePoint.Transform.gameObject);
        Resources.Remove(resourcePoint);
        _gridPartition.Remove(resourcePoint);
    }

    public List<ResourcePoint> GetResourcesFromCircle(float circleRadius, Vector2 circleCenter)
    {
        return _gridPartition.QueryByCircle(circleRadius, circleCenter);
    }

    public List<ResourcePoint> GetResourcesFromCellDirectly(Vector2Int cellPos)
    {
        return _gridPartition.QueryDirectlyCell(cellPos);
    }

    public void Reset()
    {
        foreach (var resource in Resources)
        {
            if (resource != null)
            {
                Destroy(resource.Transform.gameObject);
            }
        }

        Resources.Clear();
        _gridPartition.Clear();
    }

    public class Factory : IFactory<Soil>
    {
        Soil _soilPrefab;

        public Factory(Soil soilPrefab)
        {
            _soilPrefab = soilPrefab;
        }

        public Soil Create()
        {
            return Instantiate(_soilPrefab);
        }
    }
}
