using Assets.Scripts;
using Assets.Scripts.Map;
using Assets.Scripts.RootS;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using static TreeEditor.TreeGroup;
using Zenject;
using Assets.Scripts.RootS.Plants;
using System.ComponentModel;



public class RootDrawSystem : ITickable, IInitializable
{
    //TODO: Separate the drawing of roots and blueprints

    [Inject]
    private PlantsModel PlantsModel { get; }

    public List<RootBlueprint> BlueprintsToDraw { get; set; } = new();

    const float _standardIncrement = 0.02f;
    const float _blueprintWidth = _standardIncrement * 0.75f;

    Dictionary<RootNode, float> rootWidths = new Dictionary<RootNode, float>();

    CancellationTokenSource _drawTickCoroutineCTS = new CancellationTokenSource();

    public void Initialize()
    {
        _drawTickCoroutineCTS = new CancellationTokenSource();
        UniTask.RunOnThreadPool(() => TickCoroutine(_drawTickCoroutineCTS.Token));
    }

    public async UniTaskVoid TickCoroutine(CancellationToken cancellationToken)
    {
        while ((cancellationToken != null)
            && (cancellationToken.IsCancellationRequested == false))
        {
            tickFlag = true;
            await UniTask.Delay(100);
        }
    }

    bool tickFlag = false;

    void ITickable.Tick()
    {
        if (tickFlag)
        {
            tickFlag = false;
            EvaluateAllWidths();

            foreach(Plant plant in PlantsModel.Plants)
            {
                for (int i = 0; i < plant.transform.childCount; i++)
                {
                    var child = plant.transform.GetChild(i);

                    if (child.name == "Roots")
                    {
                        child.GetComponent<MeshFilter>().mesh = GenerateRootMesh(plant.Roots);
                    }
                    else if (child.name == "RootBlueprints")
                    {
                        child.GetComponent<MeshFilter>().mesh = GenerateBluprintMesh();
                    }
                }
            }
        }
    }

    #region Mesh Generation
    Mesh CreateMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // Create the mesh
        Mesh rootMesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        rootMesh.RecalculateNormals();
        rootMesh.RecalculateBounds();

        return rootMesh;
    }

    Mesh GenerateRootMesh(PlantRoots plantRoots)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (var rootBase in plantRoots.Nodes)
        {
            if (rootBase.IsRootBase)
            {
                GenerateBranchMesh(rootBase, null, vertices, triangles, uvs);
            }
        }

        Mesh rootMesh = CreateMesh(vertices, triangles, uvs);

        return rootMesh;
    }

    float GetThinerPartWidth(float nodeWidth)
    {
        float crossSectionalArea = Mathf.Pow(nodeWidth, 2) * Mathf.PI;
        return Mathf.Sqrt((crossSectionalArea - _standardIncrement) / Mathf.PI);
    }

    void GenerateBranchMesh(
        RootNode node,
        RootNode Parent,
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector2> uvs)
    {
        // Fetch node width from rootWidths, default to standardIncrement if not found
        if (!rootWidths.TryGetValue(node, out float nodeWidth))
            nodeWidth = _standardIncrement;

        // Get Parent position and width from rootWidths
        Vector2 parentPos = Parent != null ? Parent.Position : node.Position;

        // Generate straight segment vertices
        Vector2 direction = (node.Position - parentPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        int baseVertexIndex = vertices.Count;

        float crossSectionalArea = Mathf.Pow(nodeWidth, 2) * Mathf.PI;
        float thinerPartWidth = Mathf.Sqrt((crossSectionalArea - _standardIncrement) / Mathf.PI);

        Vector3 v1 = parentPos + perpendicular * nodeWidth / 2;
        Vector3 v2 = parentPos - perpendicular * nodeWidth / 2;
        Vector3 v3 = node.Position + perpendicular * thinerPartWidth / 2;
        Vector3 v4 = node.Position - perpendicular * thinerPartWidth / 2;

        // Add the four vertices
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        // Create triangles for the current segment
        triangles.Add(baseVertexIndex);
        triangles.Add(baseVertexIndex + 2);
        triangles.Add(baseVertexIndex + 1);
        triangles.Add(baseVertexIndex + 1);
        triangles.Add(baseVertexIndex + 2);
        triangles.Add(baseVertexIndex + 3);

        // Add UVs
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));

        // Handle merging for nodes with multiple children
        if (node.Childs.Count > 1)
        {
            GenerateMergeCaps(node, vertices, triangles, uvs);
        }

        // Process all children nodes recursively
        foreach (var child in node.Childs)
        {
            GenerateBranchMesh(child, node, vertices, triangles, uvs);
        }
    }

    Mesh GenerateBluprintMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        foreach(var blueprint in BlueprintsToDraw)
        {
            for (int i = 0; i < blueprint.RootPath.Count; i++)
            {
                // Get Parent position and width from rootWidths
                Vector2 parentPos = i != 0 ? blueprint.RootPath[i - 1] : blueprint.RootNode.Position;

                // Generate straight segment vertices
                Vector2 direction = (blueprint.RootPath[i] - parentPos).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                int baseVertexIndex = vertices.Count;

                Vector3 v1 = parentPos + perpendicular * _blueprintWidth / 2;
                Vector3 v2 = parentPos - perpendicular * _blueprintWidth / 2;
                Vector3 v3 = blueprint.RootPath[i] + perpendicular * _blueprintWidth / 2;
                Vector3 v4 = blueprint.RootPath[i] - perpendicular * _blueprintWidth / 2;

                // Add the four vertices
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                // Create triangles for the current segment
                triangles.Add(baseVertexIndex);
                triangles.Add(baseVertexIndex + 2);
                triangles.Add(baseVertexIndex + 1);
                triangles.Add(baseVertexIndex + 1);
                triangles.Add(baseVertexIndex + 2);
                triangles.Add(baseVertexIndex + 3);

                // Add UVs
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
            }
        }

        var mesh = CreateMesh(vertices, triangles, uvs);
        return mesh;
    }

    void GenerateMergeCaps(
        RootNode node,
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector2> uvs)
    {
        if (!rootWidths.TryGetValue(node, out float mergeWidth))
            mergeWidth = _standardIncrement;

        mergeWidth = GetThinerPartWidth(mergeWidth);

        Vector2 mergeCenter = node.Position;

        int segments = 20; // Number of segments for the semi-circle (higher = smoother)
        float angleStep = Mathf.PI / segments;

        if (node.Parent is null)
            return;

        Vector2 direction = (node.Parent.Position - node.Position).normalized;

        int startIndex = vertices.Count;

        float angleOffset = Mathf.Atan2(direction.y, direction.x);

        // Generate vertices for the semi-circle
        for (int i = 0; i <= segments; i++)
        {
            float angle = angleOffset + Mathf.PI / 2 + i * angleStep; // Semi-circle angles
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * mergeWidth / 2,
                Mathf.Sin(angle) * mergeWidth / 2);

            vertices.Add(mergeCenter + offset);
            uvs.Add(new Vector2(i / (float)segments, 1)); // Optional UV mapping
        }

        // Create triangles connecting the semi-circle to the Parent root
        for (int i = 0; i < segments; i++)
        {
            triangles.Add(startIndex + i + 1);
            triangles.Add(startIndex + i);
            triangles.Add(startIndex); // Center of the semi-circle
        }
    }

    #endregion Mesh Generation

    void EvaluateAllWidths()
    {
        var PlantRootBases = PlantsModel.Plants
            .Select(p => p.Roots)
            .SelectMany(r => r.Nodes
                .Where(n => n.IsRootBase));

        foreach (var rootBase in PlantRootBases)
        {
            EvaluateNodeWidth(rootBase);
        }
    }

    float EvaluateNodeWidth(RootNode node)
    {
        float nodeArea = 0f;

        if(node.Childs is not null)
        {
            foreach (var child in node.Childs)
            {
                nodeArea += EvaluateNodeWidth(child);
            }
        }

        nodeArea += _standardIncrement;

        rootWidths[node] = Mathf.Sqrt(nodeArea / Mathf.PI);

        return nodeArea;
    }
}
