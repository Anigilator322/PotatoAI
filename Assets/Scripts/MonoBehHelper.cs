using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class MonoBehHelper : MonoBehaviour
{
    MeshFilter meshFilter;
    [Inject] PlantsModel PlantsModel;
    [Inject] VisibilitySystem VisibilitySystem;

    private void Awake()
    {
        Application.targetFrameRate = 144;
    }

    private void OnDrawGizmos()
    {
        if (PlantsModel == null)
            return;

        foreach(var plant in PlantsModel.Plants)
        {
            Gizmos.color = Color.red;

            foreach (var node in plant.Roots.Nodes)
            {
                Gizmos.DrawSphere((Vector2)node.Transform.position, 0.1f);
            }
        }

        foreach(var plantAndPoints in VisibilitySystem._visibleByPlantsPoints)
        {
            Gizmos.color = Color.blue;
            foreach (var point in plantAndPoints.Value)
            {
                Gizmos.DrawSphere((Vector2)point.Transform.position, 0.1f);
            }
        }

        foreach(var start in VisibilitySystem.Starts)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(start, 0.1f);
            foreach (var end in VisibilitySystem.Ends)
            {
                var length = (end - start).magnitude;
                float radius = 0.2f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(end,radius);
            }
        }

        if (meshFilter == null)
        {
            meshFilter = PlantsModel.Plants.First().GetComponentInChildren<MeshFilter>();
        }

        foreach(var vertice in meshFilter.mesh.vertices)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(vertice, 0.05f);
        }

    }
}
