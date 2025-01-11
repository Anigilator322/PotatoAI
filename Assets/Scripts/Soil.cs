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

    public List<ResourcePoint> GetResourcesFromCircle(float circleRadius, Vector2 circleCenter)
    {
        return _gridPartition.Query(circleRadius, circleCenter);
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
