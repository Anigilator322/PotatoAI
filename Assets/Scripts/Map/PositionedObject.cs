using UnityEngine;

namespace Assets.Scripts.Map
{
    public interface IPositionedObject
    {
        public Vector2 Position { get; set; }
    }
}
