using Assets.Scripts.Bootstrap;
using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.ReflectionBaking.Mono.Cecil;

public enum ResourceType
{
    Water,
    Nitrogen,
    Phosphorus,
    Potassium
}

public struct ResourcePoint : IPositionedObject
{
    public ResourceType ResourceType { get; set; }
    public float Amount { get; set; }
    public Vector2 Position { get; set; }

    public ResourcePoint(ResourceType resourceType, float amount, Vector2 position)
    {
        ResourceType = resourceType;
        Amount = amount;
        Position = position;
    }

    

    //public class Factory : IFactory<ResourceType, Vector2, ResourcePoint>
    //{
    //    const float AMOUNT = 10;

    //    public ResourcePoint Create(ResourceType resourceType, float amount, Vector2 position)
    //    {
    //        return new ResourcePoint(resourceType, (int)AMOUNT, position);
    //    }
    //}
}
