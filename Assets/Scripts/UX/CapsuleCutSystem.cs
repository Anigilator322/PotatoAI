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
        public CapsuleCutComponent CapsuleCutComponent;

        #region renderTools
        private Renderer _rend;
        private MaterialPropertyBlock _mpb;
        private readonly Vector2 MIN_MAP = new Vector2(-20, -20);
        private readonly Vector2 MAX_MAP = new Vector2(20, 20);
        #endregion

        private VisibilitySystem _visibilitySystem;

        public CapsuleCutSystem(Renderer capsuleCutViewRenderer, VisibilitySystem visibilitySystem)
        {
            _visibilitySystem = visibilitySystem;
            InitializeSystem(capsuleCutViewRenderer);
        }

        public void Reset()
        {
            CapsuleCutComponent?.Reset();
            CapsuleCutComponent = new CapsuleCutComponent();
            InitializeFogView(_rend);
        }

        private void InitializeSystem(Renderer capsuleCutViewRenderer)
        {
            CapsuleCutComponent = new CapsuleCutComponent();
            _visibilitySystem.OnCapsuleCreated += SetCapsule;
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
            CapsuleCutComponent.CapsulesFormated.Add(new VisibilityCapsule(startUV, endUV, normalizedRadius));
            UpdateVisionShader();
        }

        public void UpdateVisionShader()
        {
            CapsuleCutComponent.CapsuleDatas.Clear();
            if (CapsuleCutComponent.CapsulesFormated is null)
                return;
            if (CapsuleCutComponent.CapsulesFormated.Count() == 0)
                return;

            foreach (var rootEdge in CapsuleCutComponent.CapsulesFormated)
            {
                Vector2 start = rootEdge.Start;
                Vector2 end = rootEdge.End;
                float radius = rootEdge.Radius;

                CapsuleCutComponent.CapsuleDatas.Add(new CapsuleData()
                {
                    Points = new Vector4(start.x, start.y, end.x, end.y),
                    Extra = new Vector4(radius, 0, 0, 0)
                });
            }
            if (CapsuleCutComponent.CapsuleDatas?.Count() == 0)
                return;

            CapsuleCutComponent.CapsuleBuffer?.Release();
            CapsuleCutComponent.CapsuleBuffer?.Dispose();
            CapsuleCutComponent.CapsuleBuffer = new ComputeBuffer(CapsuleCutComponent.CapsuleDatas.Count() != 0 ? CapsuleCutComponent.CapsuleDatas.Count() : 1, 32);
            CapsuleCutComponent.BufferCapacity = CapsuleCutComponent.CapsulesFormated.Count;
            CapsuleCutComponent.CapsuleBuffer.SetData(CapsuleCutComponent.CapsuleDatas);

            _mpb.SetBuffer("capsuleBuffer", CapsuleCutComponent.CapsuleBuffer);
            _mpb.SetInt("_CapsuleCount", CapsuleCutComponent.CapsuleDatas.Count());
            _mpb.SetVector("_MapMin", MIN_MAP);
            _mpb.SetVector("_MapMax", MAX_MAP);
            _rend.SetPropertyBlock(_mpb);
            
        }
    }
}
