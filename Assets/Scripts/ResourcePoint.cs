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
}
