using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UX
{
    public class CapsuleCutSystem : MonoBehaviour
    {
        public Material fogMaterial; // Материал с вашим шейдером
        private List<Vector4> capsuleSegments = new List<Vector4>();
        private List<float> capsuleRadii = new List<float>();
        public List<(Vector2 start, Vector2 end, float radius)> Capsules { get; set; } = new List<(Vector2 start, Vector2 end, float radius)>();
        private const int MAX_CAPSULES = 10; // Должно совпадать с размером массива в шейдере

        Renderer rend;
        MaterialPropertyBlock mpb;

        private Vector2 minMap = new Vector2(-10, -5);
        private Vector2 maxMap = new Vector2(10, 5);

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            mpb = new MaterialPropertyBlock();
        }
        public void SetCapsule(Vector2 start, Vector2 end, float radius)
        {
            if(Capsules.Count() > MAX_CAPSULES)
            {
                Debug.LogError("Too many capsules");
                return;
            }

            Vector2 startUV = (start - minMap) / (maxMap - minMap);
            Vector2 endUV = (end - minMap) / (maxMap - minMap);

            // Преобразуем радиус: если радиус задан в мировых единицах, нормализуем его относительно размера карты по оси X или Y
            float normalizedRadius = radius / (maxMap.x - minMap.x);
            Capsules.Add((startUV, endUV, normalizedRadius));
        }

        public void Update()
        {
            // Очистим списки перед заполнением
            capsuleSegments.Clear();
            capsuleRadii.Clear();
            if (Capsules is null)
                return;
            if(Capsules.Count() == 0)
                return;
            
            // Получаем актуальные грани корней (замените этот код на ваш)
            foreach (var rootEdge in Capsules)
            {
                Vector2 start = rootEdge.start;
                Vector2 end = rootEdge.end;
                float radius = rootEdge.radius;

                capsuleSegments.Add(new Vector4(start.x, start.y, end.x, end.y));
                capsuleRadii.Add(radius);

                // Ограничение по количеству капсул
                if (capsuleSegments.Count >= MAX_CAPSULES)
                    break;
            }

            // Заполняем пустыми значениями, если капсул меньше MAX_CAPSULES
            while (capsuleSegments.Count < MAX_CAPSULES)
            {
                capsuleSegments.Add(Vector4.zero);
                capsuleRadii.Add(0f);
            }

            // Передаём данные в шейдер
            mpb.SetInt("_CapsuleCount", Capsules.Count());
            mpb.SetVectorArray("_Capsules", capsuleSegments);
            mpb.SetFloatArray("_CapsuleRadius", capsuleRadii);
            fogMaterial.SetVector("_MapMin", minMap);
            fogMaterial.SetVector("_MapMax", maxMap);

            rend.SetPropertyBlock(mpb);
            Debug.Log("Capsules updated");
        }
    }
}
