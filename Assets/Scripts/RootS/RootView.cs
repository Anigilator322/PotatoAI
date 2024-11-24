using Assets.Scripts.RootS.Plants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.RootS
{
    public class RootView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private List<Vector2> _linePath = new List<Vector2>();
        [SerializeField ]private Plant _plant;



        public void GrowUp(RootNode rootNode)
        {
            _linePath.Add(rootNode.Position);
            StartCoroutine(GrowUpEnumerator());
        }    

        private IEnumerator GrowUpEnumerator()
        {
            //TODO SmoothUpdating lineRenderer
            yield return new WaitForSeconds(1f);
        }

    }
}
