using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RootS
{
    public class RootSegmentDrawer : MonoBehaviour
    {
        public LineRenderer lineRendererPrefab; // LineRenderer prefab
        public float segmentLength = 0.5f;     // Lenght of 1 segment
        public float maxAngle = 30f;           // max Angle derivation between segments
        public int nSegmentsForAngle = 3;      // Count of segments for angle
        public Camera mainCamera;              // main camera link

        private RootGraph _rootGraph;
        private List<List<Vector3>> allRoots = new List<List<Vector3>>(); // Roots list
        private LineRenderer currentLineRenderer; // current Line Renderer that drawing
        private List<Vector3> currentRootSegments = new List<Vector3>();  // current building root
        private bool isDrawing = false;        // flag for drawing
        private bool isBlocked = false;        // flag for block drawing

        void Start()
        {
            _rootGraph = new RootGraph();
        }
        void Update()
        {
            // pressed LMB
            if (Input.GetMouseButtonDown(0) && !isBlocked)
            {
                TryBlueprint();
            }

            // if drawing and not blocked
            if (isDrawing && !isBlocked)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.nearClipPlane));
                worldPos.z = 0;

                // if mouse too far from last segment - then add new segment
                if (Vector3.Distance(currentRootSegments[currentRootSegments.Count - 1], worldPos) > segmentLength)
                {
                    AddSegment(worldPos);
                }
            }

            // LBM released then finish root
            if (Input.GetMouseButtonUp(0))
            {
                FinishCurrentRoot();
            }
        }
        private void TryBlueprint()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.nearClipPlane));
            worldPos.z = 0;

            Node potentialParent = _rootGraph.FindClosestNode(worldPos, segmentLength);
            if (potentialParent == null)
            {
                _rootGraph.AddRootNode(worldPos);  //if parent not found then add new root
                StartNewRoot(worldPos);
            }
            else
            {
                _rootGraph.AddNode(worldPos, potentialParent);
                if (_rootGraph.isNodeLast(potentialParent))
                {
                    //continue root
                    ContinueRoot(potentialParent);
                }
                else
                {
                    //StartingNewSubRoot;

                }
                //_rootGraph.AddNode(worldPos, potentialParent);
            }
        }

        private void StartNewSubRoot(Node lastNode)
        {
            currentRootSegments.Clear(); // clear old segments list
            currentRootSegments.Add(lastNode.Position); // add new point
            isDrawing = true;
            isBlocked = false;

            // creating new linerenderer with 1 point
            currentLineRenderer = Instantiate(lineRendererPrefab);
            currentLineRenderer.positionCount = 1;
            currentLineRenderer.SetPosition(0, lastNode.Position);
        }

        //TODO: Gets old segments from graph
        private void ContinueRoot(Node lastNode)
        {
            currentRootSegments.Clear(); // clear old segments list
            currentRootSegments.Add(lastNode.Position); // add new point
            isDrawing = true;
            isBlocked = false;

            // creating new linerenderer with 1 point
            currentLineRenderer = Instantiate(lineRendererPrefab);
            currentLineRenderer.positionCount = 1;
            currentLineRenderer.SetPosition(0, lastNode.Position);
        }

        private void StartNewRoot(Vector3 worldPos)
        {
            

            currentRootSegments.Clear(); // clear old segments list
            currentRootSegments.Add(worldPos); // add new point
            isDrawing = true;
            isBlocked = false;

            // creating new linerenderer with 1 point
            currentLineRenderer = Instantiate(lineRendererPrefab);
            currentLineRenderer.positionCount = 1;
            currentLineRenderer.SetPosition(0, worldPos);
        }

        // finish current root
        void FinishCurrentRoot()
        {
            if (isDrawing)
            {
                allRoots.Add(new List<Vector3>(currentRootSegments)); // saving current root
                isDrawing = false;
                isBlocked = false;
            }
        }

        void AddSegment(Vector3 targetPosition)
        {
            Vector3 lastSegment = currentRootSegments[currentRootSegments.Count - 1];
            Vector3 direction = (targetPosition - lastSegment).normalized;

            // angle derivation
            if (currentRootSegments.Count > 1)
            {
                Vector3 averageDirection = GetAverageDirection(nSegmentsForAngle);

                // calculate angle between segments and average direction in degrees
                float angle = Vector3.Angle(averageDirection, direction);

                // if angle too big - then block drawing
                if (angle > maxAngle)
                {
                    isBlocked = true; // block drawing
                    Debug.Log("Превышен угол. Блокировка рисования.");
                    return;
                }
            }

            // if angle allowed then drawing
            Vector3 newSegment = lastSegment + direction * segmentLength;
            currentRootSegments.Add(newSegment);

            // update current lineRenderer
            currentLineRenderer.positionCount = currentRootSegments.Count;
            currentLineRenderer.SetPositions(currentRootSegments.ToArray());
        }

        Vector3 GetAverageDirection(int n)
        {
            int count = Mathf.Min(n, currentRootSegments.Count - 1);
            Vector3 sumDirections = Vector3.zero;

            // summ direction of segments
            for (int i = currentRootSegments.Count - 1; i > currentRootSegments.Count - 1 - count; i--)
            {
                Vector3 segmentDirection = (currentRootSegments[i] - currentRootSegments[i - 1]).normalized;
                sumDirections += segmentDirection;
            }

            // averaging
            return (sumDirections / count).normalized;
        }
    }
}

