using Assets.Scripts.Map;
using Assets.Scripts.Roots;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class VisibilitySystem
    {
        private GridPartition<RootNode> _gridPartition;

        private void CapsuleCast(Vector2 start, Vector2 end, float radius)
        {
            //Search cells in capsule
            //Return List<Cell> with all cells in capsule
            var cells = _gridPartition.GetCellsInCapsule(start, end, radius);
        }

        public void UpdateVisibilityForRootNode(RootNode revealer)
        {
            var edge = revealer.Position - revealer.Parent.Position;
            int revealRadius = ((int)revealer.Type);//Make configuration for revealRadius of different rootTypes
            float length = edge.magnitude;
            float width = revealRadius * 2;
            //var area = CapsuleCast()
            // Make BFS from revealer to all cells in capsule by quadrants. From 0 to 2PI
            // Make BFS from revealer.Parent to all cells in capsule by quadrants. From 0 to 2PI
            // Mark all gained cells as visible
            // Notify Cells about visibility change
        }

        public void UpdateVisibilityForResourcePoint()
        {

        }
    }
}
