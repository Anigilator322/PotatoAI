using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    public interface IRootBlueprint : IIdentifiable
    {
        IReadOnlyList<Vector2> RootPath { get; }
        RootType RootType { get; set; }
        RootNode StartRootNode { get; set; }

        void AppendPoint(Vector2 pathPoint);
        bool TryRemoveLastPoint();
    }
}