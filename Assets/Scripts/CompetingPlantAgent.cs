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

public class CompetingPlantAgent : Agent
{
    [SerializeField]
    DecisionRequester decisionRequester;

    [SerializeField]
    SceneContext sceneContextPrefab;

    SceneContext sceneContext;

    Plant plant;

    public override void OnEpisodeBegin()
    {
        sceneContext = Instantiate(sceneContextPrefab, transform);
        sceneContext.Run();

        plant = sceneContext.Container.Resolve<PlayerDataModel>().playersPlant;

        base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(12);
        var nodesPositions = plant.Roots.Nodes
            .SelectMany(node => new List<float> { node.Transform.position.x, node.Transform.position.y, node.Transform.position.z })
            .ToList();

        sensor.AddObservation(nodesPositions);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        Debug.Log(actions.DiscreteActions[0]);
        Debug.Log(actions.ContinuousActions[0]);
        Debug.Log(actions.ContinuousActions[1]);
    }

    public void EndOfGame()
    {
        EndEpisode();
        Destroy(sceneContext);
    }
}
