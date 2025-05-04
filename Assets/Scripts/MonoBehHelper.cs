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
    [SerializeField] 
    private CapsuleCutSystem _capsuleCutSystem;
    #region DrawGizmosFields
    MeshFilter meshFilter;
    [Inject] 
    private PlantsModel _plantsModel;
    [Inject] 
    private VisibilitySystem _visibilitySystem;
    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 144;
    }

    private void OnDrawGizmos()
    {
        if (_plantsModel == null)
            return;
        DrawGizmosForFOV();
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
    [SerializeField]
    bool drawGizmosForFOV = true;

    private void DrawGizmosForFOV()
    {
        if (!drawGizmosForFOV)
            return;

        foreach (var plantAndPoints in _visibilitySystem.VisibleByPlantsPoints)
        {
            Gizmos.color = Color.blue;
            foreach (var point in plantAndPoints.Value)
            {
                Gizmos.DrawSphere((Vector2)point.Transform.position, 0.1f);
            }
        }
        foreach (var capsule in _visibilitySystem.VisibilityCapsules)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(capsule.Start, capsule.Radius);
            var length = (capsule.End - capsule.Start).magnitude;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(capsule.End, capsule.Radius);
        }
    }
    #endregion
}
