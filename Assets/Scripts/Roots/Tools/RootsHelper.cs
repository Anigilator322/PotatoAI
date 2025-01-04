using Assets.Scripts.Roots.Plants;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.Tools
{
    public static class RootsHelper
    {        
        public static void AddFancyRootNodes(PlantRoots plantRoots, RootSpawnSystem rootSpawnSystem)
        {
            const int depth = 4; // Number of levels
            const float segmentLength = 1f; // Vertical distance between levels
            const float branchSpacing = 0.5f; // Horizontal spacing between nodes

            List<RootNode> allNodes = new List<RootNode>();

            for (int level = 0; level < depth; level++)
            {
                int nodesInLevel = (int)Mathf.Pow(2, level); // Number of nodes at this level
                float levelY = -level * segmentLength; // Y position for this level
                float startX = -((nodesInLevel - 1) * branchSpacing) / 2; // Center the level horizontally

                for (int i = 0; i < nodesInLevel; i++)
                {
                    float x = startX + i * branchSpacing;

                    RootNode parent = null;

                    // Link to the Parent node
                    if (level > 0) // Skip root level
                    {
                        int parentIndex = (i / 2) + ((int)Mathf.Pow(2, level - 1) - 1); // Calculate Parent index
                        parent = allNodes[parentIndex];
                    }

                    RootNode node = new RootNode(
                        new Vector2(x, levelY),
                        parent,
                        RootType.Harvester
                    );

                    // Add to the overall list of nodes
                    allNodes.Add(node);
                }
            }

            foreach (var rootNode in allNodes)
                rootSpawnSystem.SpawnRootNode(plantRoots, rootNode);
        }
    }
}
