using Assets.Scripts.FogOfWar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UX
{
    public class CapsuleCutComponent
    {
       
        #region CapsuleCutSystem
        public List<VisibilityCapsule> CapsulesFormated;
        public ComputeBuffer CapsuleBuffer;
        public List<CapsuleData> CapsuleDatas;
        public int BufferCapacity = 0;
        #endregion
        public CapsuleCutComponent()
        {
            CapsulesFormated = new List<VisibilityCapsule>();
            CapsuleDatas = new List<CapsuleData>();
        }
        ~CapsuleCutComponent()
        {
            if (CapsuleBuffer != null)
            {
                CapsuleBuffer.Release();
                CapsuleBuffer.Dispose();
                CapsuleBuffer = null;
            }
        }

        public void Reset()
        {
            CapsuleBuffer?.Release();
            CapsuleBuffer?.Dispose();
            CapsuleBuffer = null;
            CapsulesFormated.Clear();
            CapsuleDatas.Clear();
            BufferCapacity = 0;
        }
    }
}
