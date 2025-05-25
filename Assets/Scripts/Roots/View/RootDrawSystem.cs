
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using Assets.Scripts.UX;
using static UnityEngine.Mesh;
using ModestTree;
using Assets.Scripts.Roots.RootsBuilding.Growing;

class ModifiableMeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<Vector3> normals = new List<Vector3>();


    // Track min/max UV values
    public Vector2 uvMin = new Vector2(float.MaxValue, float.MaxValue);
    public Vector2 uvMax = new Vector2(float.MinValue, float.MinValue);

    public void AddUV(Vector2 uv)
    {
        uvs.Add(uv);
        // Update bounds
        uvMin = Vector2.Min(uvMin, uv);
        uvMax = Vector2.Max(uvMax, uv);
    }

    public void NormalizeUVs()
    {
        Vector2 uvRange = uvMax - uvMin;

        for (int i = 0; i < uvs.Count; i++)
        {
            Vector2 uv = uvs[i];
            // Normalize to 0-1 range
            uv.x = (uv.x - uvMin.x) / uvRange.x;
            uv.y = (uv.y - uvMin.y) / uvRange.y;
            uvs[i] = uv;
        }
    }
}

struct RectangleVetrices
{
    //indices of vertexes
    public int upLeft;
    public int upRight;
    public int downLeft;
    public int downRight;
}

namespace Assets.Scripts.Roots.View
{
    public class RootDrawSystem : ITickable, IInitializable
    {
        //TODO: outsource config value setup
        const float _standardIncrement = 0.01f;
        const float _blueprintWidth = _standardIncrement * 6f;

        [Inject] private PlantsModel PlantsModel { get; }
        [Inject] private PlayerDataModel PlayerDataModel { get; }
        [Inject] private MeshCache MeshCache { get; }
        [Inject] private GrowingRootsModel GrowingRoots { get; }

        private Dictionary<RootNode, float> _rootWidths = new();

        CancellationTokenSource _drawTickCoroutineCTS = new();
        public async void Initialize()
        {
            _drawTickCoroutineCTS = new CancellationTokenSource();
            await UniTask.RunOnThreadPool(() => TickTimerCoroutine(_drawTickCoroutineCTS.Token));
        }
    
        ~RootDrawSystem()
        {
            if(_drawTickCoroutineCTS is not null)
            {
                _drawTickCoroutineCTS.Cancel();
                _drawTickCoroutineCTS.Dispose();
                _drawTickCoroutineCTS = null;
            }
        }

        public async UniTaskVoid TickTimerCoroutine(CancellationToken cancellationToken)
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

                UpdateBluprintsMeshes();

                foreach (Plant plant in PlantsModel.Plants)
                {
                    UpdateRootsMeshes(plant.Roots);
                }
            }
        }

        #region Mesh Generation

        /// <summary>
        /// Обновить меши корневой системы
        /// </summary>
        /// <param name="plantRoots"></param>
        void UpdateRootsMeshes(PlantRoots plantRoots)
        {
            var baseNode = plantRoots.Nodes.Single(node => node.IsRootBase);

            var rootsMeshesData = MeshCache.meshFilters[plantRoots.plant]
                .ToDictionary(
                    meshFilter => meshFilter.Key,
                    meshFilter => new ModifiableMeshData());

            rootsMeshesData.Remove(RootType.Blueprint);

            var rectangleVertices = new RectangleVetrices();
            rectangleVertices.downLeft = rectangleVertices.upLeft = rectangleVertices.downLeft = rectangleVertices.downRight = -1;

            GenerateBranchMeshData(baseNode, rectangleVertices, rootsMeshesData);

            foreach (var rootMeshData in rootsMeshesData)
            {
                UpdateMeshFilter(MeshCache.meshFilters[plantRoots.plant][rootMeshData.Key],
                    rootMeshData.Value.vertices,
                    rootMeshData.Value.triangles,
                    rootMeshData.Value.uvs,
                    rootMeshData.Value.normals);
            }
        }

        /// <summary>
        /// Обновить меш чертежей корней
        /// </summary>
        void UpdateBluprintsMeshes()
        {
            var plantBlueprints = PlantsModel.Plants
                .ToDictionary(
                    plant => plant,
                    plant => new List<RootBlueprint>());
                
            foreach (var growingRoot in GrowingRoots.Blueprints.Values)
            {
                plantBlueprints[growingRoot.Plant].Add(growingRoot.Blueprint);
            }

            if(PlayerDataModel.CurrentlyBeingDrawnBlueprint is not null)
            {
                plantBlueprints[PlayerDataModel.playersPlant].Add(PlayerDataModel.CurrentlyBeingDrawnBlueprint.blueprint);
            }

            foreach (var group in plantBlueprints)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                List<Vector2> uvs = new List<Vector2>();

                foreach (var blueprint in group.Value)
                {
                    for (int i = 0; i < blueprint.RootPath.Count; i++)
                    {
                        // Get Parent position and width from rootWidths
                        Vector2 parentPos = i != 0 ? blueprint.RootPath[i - 1] : blueprint.StartRootNode.Transform.position;

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
                        uvs.Add(CalculateUVFromPosition(v1));
                        uvs.Add(CalculateUVFromPosition(v2));
                        uvs.Add(CalculateUVFromPosition(v3));
                        uvs.Add(CalculateUVFromPosition(v4));
                    }
                }

                var meshFilter = MeshCache.GetMeshFilter(group.Key, RootType.Blueprint);

                
                UpdateMeshFilter(meshFilter, vertices, triangles, uvs);
            }
        }

        void GenerateBranchMeshData(
            RootNode node,
            RectangleVetrices parentSegmentRectangle,
            Dictionary<RootType, ModifiableMeshData> rootsMeshesData = null)
        {
            var meshData = rootsMeshesData[node.Type];

            var vertices = meshData.vertices;
            var triangles = meshData.triangles;
            var uvs = meshData.uvs;
            var normals = meshData.normals;

            var Parent = node.Parent;

            if (Parent is not null)
            {
                var nodeWidth = _rootWidths[node];

                // Get Parent position
                Vector3 parentPos = Parent.Transform.position;

                // Generate straight segment vertices
                Vector3 direction = (node.Transform.position - parentPos);
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

                var normalizedDirection = direction.normalized;
                var normalizedperpendicular = perpendicular.normalized;

                float crossSectionalArea = Mathf.Pow(nodeWidth, 2) * Mathf.PI;

                float widerPartWidth = Mathf.Sqrt((crossSectionalArea + _standardIncrement) / Mathf.PI);

                var thinerPartOffest = perpendicular * ((nodeWidth / 2) / perpendicular.magnitude);

                Vector3 v1, v2;
                Vector3 v3 = node.Transform.position + thinerPartOffest;
                Vector3 v4 = node.Transform.position - thinerPartOffest;

                //v1.z = v2.z = v3.z = v4.z = zOffset;
                //zOffset += zStep;

                if (Parent.Childs.Count > 1 || Parent.Parent is null)
                {
                    //Первый прямоугольник корешка
                    var widerPartOffest = perpendicular * ((widerPartWidth / 2) / perpendicular.magnitude);
                    v1 = parentPos + widerPartOffest;
                    v2 = parentPos - widerPartOffest;

                    parentSegmentRectangle.upRight = vertices.Count;
                    vertices.Add(v1);
                    normals.Add(Vector3.zero);
                    parentSegmentRectangle.upLeft = vertices.Count;
                    vertices.Add(v2);
                    normals.Add(Vector3.zero);
                    meshData.AddUV(CalculateUVFromPosition(v1));
                    meshData.AddUV(CalculateUVFromPosition(v2));
                }
                else
                {
                    if (parentSegmentRectangle.upRight == -1)
                        throw new Exception("lastAddedRectangle is not set");

                    var angle = Vector2.SignedAngle(direction, parentPos - Parent.Parent.Transform.position);

                    if (Mathf.Abs(angle) < 5)
                    {
                        parentSegmentRectangle.upRight = parentSegmentRectangle.downRight;
                        parentSegmentRectangle.upLeft = parentSegmentRectangle.downLeft;
                    }
                    else
                    {
                        widerPartWidth = (vertices[parentSegmentRectangle.downLeft]
                            - vertices[parentSegmentRectangle.downRight])
                            .magnitude;

                        var widerPartOffest = perpendicular * ((widerPartWidth / 2) / perpendicular.magnitude);
                        v1 = parentPos + widerPartOffest;
                        v2 = parentPos - widerPartOffest;

                        if (angle > 0)
                        {
                            var projectedPoint = FindIntersection(v2, v4,
                                vertices[parentSegmentRectangle.upLeft], vertices[parentSegmentRectangle.downLeft]);

                            vertices[parentSegmentRectangle.downLeft] = projectedPoint;

                            vertices.Add(v1);
                            normals.Add(Vector3.zero);
                            meshData.AddUV(CalculateUVFromPosition(v1));

                            triangles.Add(parentSegmentRectangle.downRight);
                            triangles.Add(vertices.Count - 1);
                            triangles.Add(parentSegmentRectangle.downLeft);

                            parentSegmentRectangle.upRight = vertices.Count - 1;
                            parentSegmentRectangle.upLeft = parentSegmentRectangle.downLeft;
                        }
                        else
                        {
                            var projectedPoint = FindIntersection(v1, v3,
                                vertices[parentSegmentRectangle.upRight], vertices[parentSegmentRectangle.downRight]);

                            vertices[parentSegmentRectangle.downRight] = projectedPoint;

                            vertices.Add(v2);
                            normals.Add(Vector3.zero);
                            meshData.AddUV(CalculateUVFromPosition(v2));

                            triangles.Add(parentSegmentRectangle.downLeft);
                            triangles.Add(parentSegmentRectangle.downRight);
                            triangles.Add(vertices.Count - 1);

                            parentSegmentRectangle.upRight = parentSegmentRectangle.downRight;
                            parentSegmentRectangle.upLeft = vertices.Count - 1;
                        }
                    }
                }

                parentSegmentRectangle.downRight = vertices.Count;
                vertices.Add(v3);
                normals.Add(Vector3.zero);

                parentSegmentRectangle.downLeft = vertices.Count;
                vertices.Add(v4);
                normals.Add(Vector3.zero);

                meshData.AddUV(CalculateUVFromPosition(v3));
                meshData.AddUV(CalculateUVFromPosition(v4));

                // Create triangles for the current segment
                triangles.Add(parentSegmentRectangle.upRight);
                triangles.Add(parentSegmentRectangle.downRight);
                triangles.Add(parentSegmentRectangle.downLeft);

                triangles.Add(parentSegmentRectangle.upLeft);
                triangles.Add(parentSegmentRectangle.upRight);
                triangles.Add(parentSegmentRectangle.downLeft);

                var normalValues = CalculateQuadNormals(
                    vertices[parentSegmentRectangle.upLeft],
                    vertices[parentSegmentRectangle.upRight],
                    vertices[parentSegmentRectangle.downLeft],
                    vertices[parentSegmentRectangle.downRight]);

                normals[parentSegmentRectangle.upLeft] = normalValues[0];
                normals[parentSegmentRectangle.upRight] = normalValues[1];
                normals[parentSegmentRectangle.downLeft] = normalValues[2];
                normals[parentSegmentRectangle.downRight] = normalValues[3];
            }

            if (node.Childs.Count > 1)
            {
                GenerateMergeCapsMeshData(node, meshData);
            }

            // Process all children nodes recursively
            foreach (var child in node.Childs)
            {
                GenerateBranchMeshData(child, parentSegmentRectangle, rootsMeshesData);
            }
        }

        void GenerateMergeCapsMeshData(
            RootNode node,
            ModifiableMeshData meshData)
        {
            List<Vector3> vertices = meshData.vertices;
            List<int> triangles = meshData.triangles;
            List<Vector2> uvs = meshData.uvs;
            var normals = meshData.normals;

            if (!_rootWidths.TryGetValue(node, out float mergeWidth))
                mergeWidth = _standardIncrement;

            mergeWidth = GetThinerPartWidth(mergeWidth);

            Vector2 mergeCenter = node.Transform.position;

            int segments = 20; // Number of segments for the semi-circle (higher = smoother)
            float angleStep = Mathf.PI / segments;

            if (node.Parent is null)
                return;

            Vector2 direction =
                ((Vector2)node.Parent.Transform.position - mergeCenter)
                .normalized;

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
                normals.Add(Vector3.forward);
                meshData.AddUV(mergeCenter + offset); // Optional UV mapping
            }

            // Create triangles connecting the semi-circle to the Parent root
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(startIndex + i + 1);
                triangles.Add(startIndex + i);
                triangles.Add(startIndex); // Center of the semi-circle
            }
        }

        float GetThinerPartWidth(float nodeWidth)
        {
            float crossSectionalArea = Mathf.Pow(nodeWidth, 2) * Mathf.PI;
            return Mathf.Sqrt((crossSectionalArea - _standardIncrement) / Mathf.PI);
        }

        void UpdateMeshFilter(MeshFilter meshFilter, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals = null)
        {
            if (meshFilter is null)
                return;

            if(meshFilter.mesh is null)
                meshFilter.mesh = new Mesh();   

            var mesh = meshFilter.mesh;
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            if(normals is not null )
                mesh.normals = normals.ToArray();
            else
                mesh.RecalculateNormals();

            mesh.RecalculateBounds();
        }

        Vector2 CalculateUVFromPosition(Vector3 position)
        {
            return new Vector2(position.x, position.y);
        }

        Vector3[] CalculateQuadNormals(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 normal0 = (v0 - v1).normalized;
            Vector3 normal1 = -normal0;
            Vector3 normal2 = normal0;
            Vector3 normal3 = -normal0;

            normal0.z = normal1.z = normal2.z = normal3.z = -0.01f;

            return new Vector3[] { normal0, normal1, normal2, normal3 };
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
                    nodeArea += _standardIncrement;
                }
            }

            _rootWidths[node] = Mathf.Sqrt(nodeArea / Mathf.PI);

            return nodeArea;
        }

        //TODO: decompose into helper class
        public static Vector2 FindIntersection(Vector2 start_1, Vector2 end_1, Vector2 start_2, Vector2 end_2)
        {
            double A1 = end_1.y - start_1.y;
            double B1 = start_1.x - end_1.x;
            double C1 = start_1.x * end_1.y - end_1.x * start_1.y;

            double A2 = end_2.y - start_2.y;
            double B2 = start_2.x - end_2.x;
            double C2 = start_2.x * end_2.y - end_2.x * start_2.y;

            //Gauss solution
            double coef = -(A2 / A1);

            B2 += B1 * coef;
            C2 += C1 * coef;

            double y = C2 / B2;
            double x = (C1 - B1 * y) / A1;

            return new Vector2((float)x, (float)y);
        }
    }
}
