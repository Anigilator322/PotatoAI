using Assets.Scripts.RootS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class RootSystem : IPositionedObjects
    {
        public List<RootNode> Nodes = new List<RootNode>(); 
        
        public Vector2 GetPositionById(int index)
        {
            return Nodes[index].Position;
        }
    }
}
