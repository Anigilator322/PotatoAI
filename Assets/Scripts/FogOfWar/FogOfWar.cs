using Assets.Scripts.RootS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    //public List<Polygon> GenerateExploredPolygons(RootNode root)
    //{
    //    List<Capsule> fovCapsules = new List<Capsule>();

    //    // Traverse the existing tree and create capsules for each segment
    //    TraverseTreeAndCreateCapsules(root, fovCapsules);

    //    // Merge capsules into polygons (requires a geometry library)
    //    List<Polygon> exploredPolygons = MergeCapsulesIntoPolygons(fovCapsules);

    //    return exploredPolygons;
    //}

    //private void TraverseTreeAndCreateCapsules(RootNode node, List<Capsule> fovCapsules)
    //{
    //    foreach (var child in node.Childrens)
    //    {
    //        // Define FOV radius based on root type
    //        float radius = GetFOVRadius(node.type);

    //        // Create a capsule from this node to the child
    //        Capsule capsule = new Capsule(node.Position, child.Position, radius);
    //        fovCapsules.Add(capsule);

    //        // Recursively process children
    //        TraverseTreeAndCreateCapsules(child, fovCapsules);
    //    }
    //}

}
