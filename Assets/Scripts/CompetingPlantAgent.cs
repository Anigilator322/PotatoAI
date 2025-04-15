using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Assets.Scripts.Roots.Plants;
using System.Linq;
using System;

using Zenject;
using Zenject.Asteroids;

public class CompetingPlantAgent : Agent
{

    [Inject]
    public void Init(PlantsModel plantsModel)
    {
        _plantsModel = plantsModel;
    }

    PlantsModel _plantsModel;

    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(12);
            //_playerPlant.Roots.Nodes
            //    .SelectMany(node => new List<float> { node.Transform.position.x, node.Transform.position.y, node.Transform.position.z })
            //    .ToList());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        Debug.Log(actions.DiscreteActions[0]);
    }
}
