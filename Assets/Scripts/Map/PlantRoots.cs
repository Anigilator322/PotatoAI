using Assets.Scripts.RootS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class PlantRoots : IPositionedObjects
    {
        public List<RootNode> Nodes = new List<RootNode>(); 
        
        public void CreateNode(int parentInd, Vector2 pos)
        {
            RootNode node = new RootNode(pos,parentInd);
            Nodes[parentInd].SetChildren(Nodes.Count);
            Nodes.Add(node);
        }


        public Vector2 GetPositionById(int index)
        {
            return Nodes[index].Position;
        }
    }
}
