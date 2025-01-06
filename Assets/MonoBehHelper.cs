using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class MonoBehHelper : MonoBehaviour
{
    MeshFilter meshFilter;
    [Inject] PlantsModel PlantsModel;
    private void OnDrawGizmos()
    {
        if (PlantsModel == null)
            return;

        foreach(var plant in PlantsModel.Plants)
        {
            Gizmos.color = Color.red;

            foreach (var node in plant.Roots.Nodes)
            {
                Gizmos.DrawSphere(node.Position, 0.1f);
            }
        }

        if(meshFilter == null)
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
