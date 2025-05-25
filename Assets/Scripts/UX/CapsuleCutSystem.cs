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
    public class CapsuleCutSystem
    {
        private CapsuleCutComponent _capsuleCutComponent;

        #region renderTools
        private Renderer _rend;
        private MaterialPropertyBlock _mpb;
        private readonly Vector2 MIN_MAP = new Vector2(-20, -20);
        private readonly Vector2 MAX_MAP = new Vector2(20, 20);
        #endregion

        public CapsuleCutSystem(Renderer capsuleCutViewRenderer)
        {
            InitializeSystem(capsuleCutViewRenderer);
        }

        private void InitializeSystem(Renderer capsuleCutViewRenderer)
        {
            _capsuleCutComponent = new CapsuleCutComponent();
            InitializeFogView(capsuleCutViewRenderer);
        }

        //Creates gameobject of FogOfWar. Create new on Reset
        private void InitializeFogView(Renderer capsuleCutRenderer)
        {
            if (capsuleCutRenderer == null)
            {
                Debug.LogError("Prefab not found");
                return;
            }
            _rend = capsuleCutRenderer;
            _mpb = new MaterialPropertyBlock();
            if(_rend is null)
            {
                Debug.LogError("Renderer not found");
                return;
            }
        }

        public void SetCapsule(VisibilityCapsule capsule)
        { 
            Vector2 startUV = (capsule.Start - MIN_MAP) / (MAX_MAP - MIN_MAP);
            Vector2 endUV = (capsule.End - MIN_MAP) / (MAX_MAP - MIN_MAP);
            float normalizedRadius = capsule.Radius / (MAX_MAP.x - MIN_MAP.x);
            _capsuleCutComponent.CapsulesFormated.Add(new VisibilityCapsule(startUV, endUV, normalizedRadius));
        }

        public void UpdateVisionShader()
        {
            _capsuleCutComponent.CapsuleDatas.Clear();
            if (_capsuleCutComponent.CapsulesFormated is null)
                return;
            if (_capsuleCutComponent.CapsulesFormated.Count() == 0)
                return;

            foreach (var rootEdge in _capsuleCutComponent.CapsulesFormated)
            {
                Vector2 start = rootEdge.Start;
                Vector2 end = rootEdge.End;
                float radius = rootEdge.Radius;

                _capsuleCutComponent.CapsuleDatas.Add(new CapsuleData()
                {
                    Points = new Vector4(start.x, start.y, end.x, end.y),
                    Extra = new Vector4(radius, 0, 0, 0)
                });
            }
            if (_capsuleCutComponent.CapsuleDatas?.Count() == 0)
                return;

            if (_capsuleCutComponent.CapsuleBuffer != null)
            {
                _capsuleCutComponent.CapsuleBuffer.Release();
                _capsuleCutComponent.CapsuleBuffer.Dispose();
                _capsuleCutComponent.CapsuleBuffer = null;
            }
            _capsuleCutComponent.CapsuleBuffer = new ComputeBuffer(_capsuleCutComponent.CapsuleDatas.Count() != 0 ? _capsuleCutComponent.CapsuleDatas.Count() : 1, 32);
            _capsuleCutComponent.BufferCapacity = _capsuleCutComponent.CapsulesFormated.Count;
            _capsuleCutComponent.CapsuleBuffer.SetData(_capsuleCutComponent.CapsuleDatas);

            _mpb.SetBuffer("capsuleBuffer", _capsuleCutComponent.CapsuleBuffer);
            _mpb.SetInt("_CapsuleCount", _capsuleCutComponent.CapsuleDatas.Count());
            _mpb.SetVector("_MapMin", MIN_MAP);
            _mpb.SetVector("_MapMax", MAX_MAP);
            _rend.SetPropertyBlock(_mpb);
        }
    }
}
