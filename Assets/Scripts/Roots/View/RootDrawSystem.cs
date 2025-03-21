
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;

using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Roots.RootsBuilding;
using System;

namespace Assets.Scripts.Roots.View
{
    public class RootDrawSystem : ITickable, IInitializable
    {
        //TODO: Separate the drawing of roots and blueprints

        [Inject]
        private PlantsModel PlantsModel { get; }

        [Inject]
        private Soil soil { get; }

        public List<RootBlueprint> BlueprintsToDraw { get; set; } = new();

        const float _standardIncrement = 0.01f;
        const float _blueprintWidth = _standardIncrement * 6f;

        Dictionary<RootNode, float> rootWidths = new Dictionary<RootNode, float>();

        CancellationTokenSource _drawTickCoroutineCTS = new CancellationTokenSource();

        public async void Initialize()
        {
            _drawTickCoroutineCTS = new CancellationTokenSource();
            await UniTask.RunOnThreadPool(() => TickCoroutine(_drawTickCoroutineCTS.Token));

            await UniTask.Delay(1000);

            var soilScale = soil.transform.localScale;

            foreach (Plant plant in PlantsModel.Plants)
            {
                for (int i = 0; i < plant.transform.childCount; i++)
                {
                    var child = plant.transform.GetChild(i);

                    if ((child.name == "Roots") ||
                        (child.name == "RootBlueprints"))
                    {
                        child.transform.localScale = new Vector3(1 / soilScale.x, 1 / soilScale.y, 1 / soilScale.z);
                    }
                }
            }
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

        float zOffset = 0f;
        float zStep = 0.01f;
        Mesh GenerateRootMesh(PlantRoots plantRoots)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            zOffset = 0f;

            foreach (var rootBase in plantRoots.Nodes)
            {
                if (rootBase.IsRootBase)
                {
                    var rectangle = new RectangleVetrices();
                    rectangle.downLeft = rectangle.upLeft = rectangle.downLeft = rectangle.downRight = -1;
                    GenerateBranchMesh(rootBase, null, vertices, triangles, uvs, rectangle);
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

        struct RectangleVetrices
        {
            //indices of vertexes
            public int upLeft;
            public int upRight;
            public int downLeft;
            public int downRight;
        }

        void GenerateBranchMesh(
            RootNode node,
            RootNode Parent,
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            RectangleVetrices parentSegmentRectangle)
        {
            if(Parent is not null)
            {
                var nodeWidth = rootWidths[node];

                // Get Parent position
                Vector3 parentPos = Parent.Transform.position;

                // Generate straight segment vertices
                Vector3 direction = (node.Transform.position - parentPos);
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
            
                var normalizedDirection = direction.normalized;
                var normalizedperpendicular = perpendicular.normalized;

                float crossSectionalArea = Mathf.Pow(nodeWidth, 2) * Mathf.PI;

                float widerPartWidth = Mathf.Sqrt((crossSectionalArea + _standardIncrement) / Mathf.PI);

                var thinerPartOffest = perpendicular * ((nodeWidth/ 2) / perpendicular.magnitude);
            
                Vector3 v3 = node.Transform.position + thinerPartOffest;
                Vector3 v4 = node.Transform.position - thinerPartOffest;

                //v1.z = v2.z = v3.z = v4.z = zOffset;
                //zOffset += zStep;

                if (Parent.Childs.Count > 1 || Parent.Parent is null)
                {
                    var widerPartOffest = perpendicular * ((widerPartWidth / 2) / perpendicular.magnitude);
                    Vector3 v1 = parentPos + widerPartOffest;
                    Vector3 v2 = parentPos - widerPartOffest;

                    parentSegmentRectangle.upRight = vertices.Count;
                    vertices.Add(v1);
                    parentSegmentRectangle.upLeft = vertices.Count;
                    vertices.Add(v2);
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                }
                else
                {
                    if(parentSegmentRectangle.upRight == -1)
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
                        Vector3 v1 = parentPos + widerPartOffest;
                        Vector3 v2 = parentPos - widerPartOffest;

                        if (angle > 0)
                        {
                            var projectedPoint = FindIntersection(v2, v4, 
                                vertices[parentSegmentRectangle.upLeft], vertices[parentSegmentRectangle.downLeft]);

                            vertices[parentSegmentRectangle.downLeft] = projectedPoint;

                            vertices.Add(v1);
                            uvs.Add(new Vector2(0, 0));

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
                            uvs.Add(new Vector2(1, 0));

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
                parentSegmentRectangle.downLeft = vertices.Count;
                vertices.Add(v4);
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                // Create triangles for the current segment
                triangles.Add(parentSegmentRectangle.upRight);
                triangles.Add(parentSegmentRectangle.downRight);
                triangles.Add(parentSegmentRectangle.downLeft);

                triangles.Add(parentSegmentRectangle.upLeft);
                triangles.Add(parentSegmentRectangle.upRight);
                triangles.Add(parentSegmentRectangle.downLeft);
            }

            if (node.Childs.Count > 1)
            {
                //zOffset += zStep;
                GenerateMergeCaps(node, vertices, triangles, uvs/*, zOffset*/);
            }

            // Process all children nodes recursively
            foreach (var child in node.Childs)
            {
                GenerateBranchMesh(child, node, vertices, triangles, uvs, parentSegmentRectangle);
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
                    nodeArea += _standardIncrement;
                }
            }

            rootWidths[node] = Mathf.Sqrt(nodeArea / Mathf.PI);

            return nodeArea;
        }

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
