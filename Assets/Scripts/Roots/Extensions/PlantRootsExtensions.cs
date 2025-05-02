using Assets.Scripts.Roots.Plants;
using Assets.Scripts.Tools;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Roots.Extensions
{
    public static class PlantRootsExtensions
    {
        public static RootNode GetNearestAllowedBasementNode(this PlantRoots plantRoots, 
            float circleRadius, 
            Vector2 circleCenter,
            RootType selectedRootType)
        {
            var nodes = plantRoots.GetNodesFromCircle(circleRadius, circleCenter)
                .Where(node => (node.Type == RootType.Harvester)
                    || (node.Type == selectedRootType));

            if (nodes.Count() == 0)
                return null;

            return Geometry.FindClosestObject(nodes, circleCenter);
        }
    }
}
