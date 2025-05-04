using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Assets.Scripts.Roots.Plants;

using Zenject;
using System.Linq;
using Assets.Scripts;
using System.Diagnostics.Contracts;
using System;

public class CompetingPlantAgent : Agent
{
    [SerializeField]
    DecisionRequester decisionRequester;

    [SerializeField]
    SceneContext sceneContext;

    PlayerDataModel playerDataModel;
    GameBootstrapper gameBootstrapper;
    TimerSystem timerSystem;

    public override void OnEpisodeBegin()
    {
        if(gameBootstrapper is null)
        {
            sceneContext.Run();
            gameBootstrapper = sceneContext.Container.Resolve<GameBootstrapper>();
            timerSystem = sceneContext.Container.Resolve<TimerSystem>();
            playerDataModel = sceneContext.Container.Resolve<PlayerDataModel>();

            timerSystem.TimeIsOverPersistent += EndOfGame;
        }
        else
        {
            gameBootstrapper.Reset();
        }

        
        base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //if (playerDataModel.playersPlant is null)
        //    return;

        //var nodesPositions = playerDataModel.playersPlant.Roots.Nodes
        //    .SelectMany(node => new List<float> { node.Transform.position.x, node.Transform.position.y, node.Transform.position.z })
        //    .ToList();

        //sensor.AddObservation(nodesPositions);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //base.OnActionReceived(actions);
        //Debug.Log(actions.DiscreteActions[0]);
        //Debug.Log(actions.ContinuousActions[0]);
        //Debug.Log(actions.ContinuousActions[1]);
    }

    public void EndOfGame()
    {
        EndEpisode();
    }
}
