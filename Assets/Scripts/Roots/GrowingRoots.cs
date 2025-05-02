using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding.Growing
{
    public class GrowingRoots
    {
        public Dictionary<string, GrowingRoot> Blueprints { get; private set; } = new();

        public void RemoveBlueprint(GrowingRoot root) => RemoveBlueprint(root.Blueprint.Id);
        public void RemoveBlueprint(string id) => Blueprints.Remove(id);
    }
}
