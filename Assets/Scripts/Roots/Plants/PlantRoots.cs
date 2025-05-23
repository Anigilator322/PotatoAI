﻿using Assets.Scripts.Map;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Roots.Plants
{
    /// <summary>
    /// A class that holds all the roots of a plant.
    /// It is supposed to be used at any moment to access/manipulate the roots of this plant
    /// </summary>
    public class PlantRoots : IIdentifiable
    {
        const int CELL_SIZE = 1;
        public string Id { get; }
        public Plant plant { get; }

        public GridPartition<RootNode> _gridPartition { get; }
        public List<RootNode> Nodes { get; }

        public PlantRoots(Plant plant)
        {
            this.plant = plant;
            Nodes = new List<RootNode>();
            _gridPartition = new GridPartition<RootNode>(CELL_SIZE);
        }

        public void AddNode(RootNode rootNode)
        {
            rootNode.Transform.parent = plant.transform;
            Nodes.Add(rootNode);
            _gridPartition.Insert(rootNode);

        }

        public List<RootNode> GetNodesFromCircle(float circleRadius, Vector2 circleCenter)
        {
            return _gridPartition.QueryByCircle(circleRadius, circleCenter);
        }

        public List<RootNode> GetNodesFromCellDirectly(Vector2Int cellPos)
        {
            return _gridPartition.QueryDirectlyCell(cellPos);
        }

        public class Factory : IFactory<Plant, PlantRoots>
        {
            public PlantRoots Create(Plant plant)
            {
                return new PlantRoots(plant);
            }
        }
    }
}
