using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationTarget : PositionedObject
{
    public ExplorationTarget(int plantId)
    {
        PlantId = plantId;
    }

    public bool IsVisible { get; set; } = false;

    public int PlantId { get; }
}
