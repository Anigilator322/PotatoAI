using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding.RootBlockingSystem
{
    public interface IBlockerNode
    {
        Transform Transform { get; }
        IBlockerNode Parent { get; }
        List<IBlockerNode> Childs { get; }
    }
}
