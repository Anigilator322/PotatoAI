using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Roots.RootsBuilding
{
    /// <summary>
    /// Обёртка вокруг RootBlueprint - Прокси, обеспечивающий удобство построения чертежа
    /// </summary>
    public class DrawingRootBlueprint : IRootBlueprint
    {
        public RootBlueprint blueprint { get; }

        List<Vector2> _pseudoRootPath = new List<Vector2>();
        public bool IsBlocked { get; set; }
        #nullable enable
        public static DrawingRootBlueprint? Create(RootType rootType, RootNode startRootNode)
        {
            if((startRootNode.Childs.Count == 0) 
                && (rootType != startRootNode.Type))
                return null;
            else
                return new DrawingRootBlueprint(rootType, startRootNode);
        }

        private DrawingRootBlueprint(RootType rootType, RootNode startRootNode)
        {
            blueprint = new RootBlueprint(rootType, startRootNode);
            if (blueprint.StartRootNode.Childs.Count == 0
                && startRootNode.Parent is not null)
            {
                _pseudoRootPath.Add(startRootNode.Parent.Transform.position);
                IsNewRoot = false;
            }
            else
            {
                IsNewRoot = true;
                Debug.Log("NEW ROOT");
            }

            _pseudoRootPath.Add(startRootNode.Transform.position);
        }

        public IReadOnlyList<Vector2> RootPath => _pseudoRootPath;

        public RootType RootType { get => blueprint.RootType; set => blueprint.RootType = value; }

        public RootNode StartRootNode { get => blueprint.StartRootNode; set => blueprint.StartRootNode = value; }

        public string Id => blueprint.Id;

        public bool IsNewRoot { get; set; }

        public void AppendPoint(Vector2 pathPoint)
        {
            blueprint.AppendPoint(pathPoint);
            _pseudoRootPath.Add(pathPoint);
        }

        public bool TryRemoveLastPoint()
        {
            if (blueprint.TryRemoveLastPoint())
            {
                _pseudoRootPath.RemoveAt(_pseudoRootPath.Count - 1);
                return true;
            }

            return false;
        }
    }
}
