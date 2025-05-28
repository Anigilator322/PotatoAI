using Assets.Scripts.FogOfWar;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using Assets.Scripts.UX;
using System;


public class MonoBehHelper : MonoBehaviour
{
    #region DrawGizmosFields
    MeshFilter meshFilter;
    [Inject] 
    private PlantsModel _plantsModel;
    //    [Inject] 
    //private VisibilitySystem _visibilitySystem;
    [Inject]
    private CapsuleCutSystem _capsuleCutSystem;
    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 144;
    }

    private void OnDrawGizmos()
    {
        if (_plantsModel == null)
            return;
        DrawGizmosForRootNodes();
        DrawGizmosForMesh();
    }

    #region DrawGizmosFuncs
    private void DrawGizmosForMesh()
    {
        if (meshFilter == null)
        {
            meshFilter = _plantsModel.Plants.First().GetComponentInChildren<MeshFilter>();
        }

        foreach (var vertice in meshFilter.mesh.vertices)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(vertice, 0.05f);
        }
    }

    private void DrawGizmosForRootNodes()
    {
        foreach (var plant in _plantsModel.Plants)
        {
            Gizmos.color = Color.red;

            foreach (var node in plant.Roots.Nodes)
            {
                Gizmos.DrawSphere((Vector2)node.Transform.position, 0.1f);
            }
        }
    }
    #endregion
    public void OnDestroy()
    {
        //_visibilitySystem.CapsuleCutSystem.CapsuleCutComponent.Dispose();
        _capsuleCutSystem.CapsuleCutComponent.Dispose();
    }
}
