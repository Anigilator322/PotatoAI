using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoilModel : MonoBehaviour
{
    public List<ResourcePoint> Resources { get; private set; } = new List<ResourcePoint>();

    // Not ideal concept separation !!!
    [SerializeField]
    public SpriteRenderer Sprite;

    const int CELL_SIZE = 1;
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

    public class Factory : IFactory<SoilModel>
    {
        SoilModel _soilPrefab;

        public Factory(SoilModel soilPrefab)
        {
            _soilPrefab = soilPrefab;
        }

        public SoilModel Create()
        {
            return Instantiate(_soilPrefab);
        }
    }
}
