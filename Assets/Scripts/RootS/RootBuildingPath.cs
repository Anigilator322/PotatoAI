using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RootS
{
    public class RootBuildingPath
    {
        private Stack<Vector2> _rootPath = new Stack<Vector2>();
        public List<Vector2> RootPath 
        {
            get
            {
                Stack<Vector2> rootPathReversed = new Stack<Vector2>(_rootPath);
                List<Vector2> result = new List<Vector2>(rootPathReversed);
                return result;
            }
            private set { }
        }


    }
}
