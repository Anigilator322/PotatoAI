using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem
{
    public interface IBlockerNode: IPositionedObject
    {
        IBlockerNode Parent { get; }
        IReadOnlyList<IBlockerNode> Childs { get; }
    }
}
