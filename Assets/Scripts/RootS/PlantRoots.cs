using Assets.Scripts.RootS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Map
{
    public class PlantRoots : IGridPartionableObjects
    {
        [Inject] private GridPartition<PlantRoots> _gridPartition;
        public List<RootNode> Nodes = new List<RootNode>();

        public Action<RootNode> OnNodeAdded;
        
        public PlantRoots(Vector2 basePosition)
        {
            //SetBaseNode(basePosition);
        }
        
        private void SetBaseNode(Vector2 basePosition)
        {
            RootNode node = new RootNode(basePosition);
            Nodes.Add(node);
            _gridPartition.Insert(0);
            OnNodeAdded?.Invoke(node);
        }
        public void CreateNode(int parentInd, Vector2 pos)
        {
            RootNode node = new RootNode(pos, parentInd);
            Nodes[parentInd].SetChildren(Nodes.Count);
            Nodes.Add(node);
            _gridPartition.Insert(Nodes.Count - 1);
            OnNodeAdded?.Invoke(node);
        }


        public Vector2 GetPositionById(int index)
        {
            return Nodes[index].Position;
        }
    }
}
