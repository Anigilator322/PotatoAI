using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public abstract class PositionedObjects<T> where T : PositionedObject
    {
        public List<T> positionedObjects;
    }
}
