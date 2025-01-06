using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;

public enum ResourceType
{
    Water,
    Nitrogen,
    Phosphorus,
    Potassium
}

public class ResourcePoint : PositionedObject
{
    public ResourceType ResourceType;
    public int amount;

    public ResourcePoint(ResourceType resourceType, int amount, Vector2 position)
    {
        ResourceType = resourceType;
        this.amount = amount;
        Position = position;
    }
}
