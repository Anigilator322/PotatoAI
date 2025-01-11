using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Sirenix.OdinInspector;
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
    
    [SerializeField]
    GameObject selectionPoint;

    [Button]
    public void SelectRootNodes()
    {
        var plantRoots = PlantsModel.Plants.First().Roots;

        //foreach(var node in plantRoots.GetNodesFromCircle(selectionPoint.transform.position, 0.5f))
        //selectionPoint.transform.position = _plant.transform.position;
    }

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
