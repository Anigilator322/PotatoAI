using Assets.Scripts.FogOfWar;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UX
{
    public struct CapsuleData
    {
        public Vector4 Points;
        public Vector4 Extra;
    }
    public class CapsuleCutSystem : MonoBehaviour
    {
        public List<VisibilityCapsule> Capsules { get; set; } = new List<VisibilityCapsule>();

    #region renderTools
        Renderer rend;
        MaterialPropertyBlock mpb;
        private ComputeBuffer capsuleBuffer;
        private Vector2 minMap = new Vector2(-20, -20);
        private Vector2 maxMap = new Vector2(20, 20);
        private int _bufferCapacity = 0;
        private List<CapsuleData> _capsuleDatas = new List<CapsuleData>();
        #endregion

        [Inject]
        private VisibilitySystem _visibilitySystem;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            mpb = new MaterialPropertyBlock();
            _visibilitySystem.OnCapsuleCreted += SetCapsule;
        }

        public void SetCapsule(VisibilityCapsule capsule)
        { 
            Vector2 startUV = (capsule.Start - minMap) / (maxMap - minMap);
            Vector2 endUV = (capsule.End - minMap) / (maxMap - minMap);
            float normalizedRadius = capsule.Radius / (maxMap.x - minMap.x);
            Capsules.Add(new VisibilityCapsule(startUV, endUV, normalizedRadius));
        }

        public void Update()
        {
            _capsuleDatas.Clear();
            if (Capsules is null)
                return;
            if(Capsules.Count() == 0)
                return;
            
            foreach (var rootEdge in Capsules)
            {
                Vector2 start = rootEdge.Start;
                Vector2 end = rootEdge.End;
                float radius = rootEdge.Radius;

                _capsuleDatas.Add(new CapsuleData() 
                {
                    Points = new Vector4(start.x, start.y, end.x, end.y),
                    Extra = new Vector4(radius, 0, 0, 0)
                });                
            }
            if(_capsuleDatas?.Count() == 0)
                return;
            if (_bufferCapacity != _capsuleDatas?.Count())
            {
                if (capsuleBuffer != null)
                {
                    capsuleBuffer.Release();
                    capsuleBuffer.Dispose();
                }
                capsuleBuffer = new ComputeBuffer(_capsuleDatas.Count() != 0 ? _capsuleDatas.Count() : 1, 32);
                _bufferCapacity = Capsules.Count;
            }
            capsuleBuffer.SetData(_capsuleDatas);
            mpb.SetBuffer("capsuleBuffer", capsuleBuffer);
            mpb.SetInt("_CapsuleCount", _capsuleDatas.Count());
            mpb.SetVector("_MapMin", minMap);
            mpb.SetVector("_MapMax", maxMap);
            rend.SetPropertyBlock(mpb);
        }

        private void OnDestroy()
        {
            if (capsuleBuffer != null)
            {
                Debug.Log("CapsuleCutSystem OnDestroy");
                capsuleBuffer.Release();
                capsuleBuffer.Dispose();
            }
        }
    }
}
