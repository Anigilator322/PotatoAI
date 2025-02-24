
using Assets.Scripts;
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

public class ResourcePoint : IPositionedObject, IDismissable
{
    public Transform Transform { get; }
    public ResourceType ResourceType { get; set; }
    public float Amount { get; set; }

    public bool IsDismissed { get; protected set; }

    public void Dismiss() => IsDismissed = true;

    public ResourcePoint(ResourceType resourceType, float amount, Vector2 position)
    {
        Transform = new GameObject().transform;
        Transform.position = position;
        Transform.parent = null;

        Transform.name = "Resource point";

        ResourceType = resourceType;
        Amount = amount;
    }
}
