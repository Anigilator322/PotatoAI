using Assets.Scripts.Map;
using Assets.Scripts.Map.Points;
using Assets.Scripts.Roots;
using Assets.Scripts.Roots.Plants;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class FieldOfView
    {
        private int _cellSize;
        public List<IPositionedObject> _visiblePoints { private set; get; }
        public List<IPositionedObject> BlockingPoints { private set; get; }
        private PlantRoots _plantRoots;
        private GridPartition<ObstaclePoint> _gridPartitionForObstaclePoint;
        private PlantsModel _model;

        public FieldOfView(PlantsModel model)
        {
            _model = model;
            _cellSize = 1;
            _visiblePoints = new List<IPositionedObject>();            
        }

        public void ComputeFov(Vector2 originPosition)
        {
            _plantRoots = _model.Plants[0].Roots;
            Vector2Int origPositionCell = GetCellCoordinates(originPosition);
            MarkVisible(origPositionCell);
            for(int i = 0; i < 4; i++)
            {
                var quadrant = new Quadrant(i,origPositionCell);
                var row = new Row { Depth = 1, StartSlope = -1, EndSlope = 1 };
                Scan(quadrant, row);
            }
        }

        private Vector2Int GetCellCoordinates(Vector2 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int y = Mathf.FloorToInt(worldPosition.y / _cellSize);
            return new Vector2Int(x, y);
        }

        private bool IsBlocking(Vector2Int position)
        {
            var points = _gridPartitionForObstaclePoint.QueryDirectlyCell(position);
            if(points.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

        private void MarkVisible(Vector2Int position)
        {
            List<RootNode> rootPoints = _plantRoots.GetNodesFromCellDirectly(position);
            List<ObstaclePoint> obstaclePoints = _gridPartitionForObstaclePoint.QueryDirectlyCell(position);
            _visiblePoints.AddRange(rootPoints);
            _visiblePoints.AddRange(obstaclePoints);
        }

        private void Reveal(Quadrant quadrant, Tile tile)
        {
            Vector2Int cell = quadrant.Transform(tile);
            MarkVisible(cell);
        }

        private bool IsWall(Quadrant quadrant, Tile tile)
        {
            if(tile.IsNull)
            {
                return false;
            }
            Vector2Int cell = quadrant.Transform(tile);
            return IsBlocking(cell);
        }

        private bool IsFloor(Quadrant quadrant, Tile tile)
        {
            if (tile.IsNull)
            {
                return false;
            }
            Vector2Int cell = quadrant.Transform(tile);
            if(cell.x < 100 || cell.y < 100 || cell.x > 100 || cell.y > 100)
            {
                return false;
            }
            return !IsBlocking(cell);
        }

        //Need to inmplement some functions as Fraction in python
        private double Slope(Tile tile)
        {
            int rowDepth = tile.Row;
            int col = tile.Col;
            return (2.0 * col - 1) / (2.0 * rowDepth);
        }

        private bool IsSymmetric(Row row, Tile tile)
        {
            int rowDepth = tile.Row;
            int col = tile.Col;
            return (col >= row.Depth * row.StartSlope && col <= row.Depth * row.EndSlope);
        }

        private void Scan(Quadrant quadrant, Row row)
        {
            Tile prevTile = new Tile 
            {
                Row = -1,
                Col = -1,
                IsNull = true
            };

            foreach(var tile in row.GetTiles())
            {
                if (IsWall(quadrant, tile) || IsSymmetric(row, tile))
                {
                    Reveal(quadrant, tile);
                }
                if (IsWall(quadrant, prevTile) && IsFloor(quadrant, tile))
                {
                    row.StartSlope = Slope(tile);
                }
                if (IsWall(quadrant, tile) && IsFloor(quadrant, prevTile))
                {
                    var nextRow = row.Next();
                    nextRow.EndSlope = Slope(tile);
                    Scan(quadrant, nextRow);
                }
                prevTile = tile;
            }
            if (IsFloor(quadrant, prevTile))
            {
                var nextRow = row.Next();
                nextRow.EndSlope = 1;
                Scan(quadrant, nextRow);
            }
        }
    }

    internal class Quadrant
    {
        internal enum QuadrantDirection
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        private readonly int _index;
        internal Vector2Int Origin;
        internal QuadrantDirection Direction => (QuadrantDirection)_index;

        internal Quadrant(int index, Vector2Int origin)
        {
            _index = index;
            Origin = origin;
        }

        internal Vector2Int Transform(Tile tile)
        {
            return Direction switch
            {
                QuadrantDirection.North => new Vector2Int(Origin.x + tile.Col, Origin.y - tile.Row),
                QuadrantDirection.South => new Vector2Int(Origin.x + tile.Col, Origin.y + tile.Row),
                QuadrantDirection.East => new Vector2Int(Origin.x + tile.Row, Origin.y + tile.Col),
                QuadrantDirection.West => new Vector2Int(Origin.x - tile.Row, Origin.y + tile.Col),
                _ => throw new ArgumentException("Invalid cardinal direction")
            };
        }
    }

    internal struct Tile
    {
        internal int Row;
        internal int Col;
        internal bool IsNull;
    }

    internal struct Row
    {
        internal int Depth;
        internal double StartSlope;
        internal double EndSlope;

        internal int RoundTiesUp(double value)
        {
            return (int)Math.Floor(value + 0.5);
        }

        internal int RoundTiesDown(double value)
        {
            return (int)Math.Ceiling(value - 0.5);
        }

        internal IEnumerable<Tile> GetTiles()
        {
            int minCol = RoundTiesUp(Depth * StartSlope);
            int maxCol = RoundTiesDown(Depth * EndSlope);

            for (int col = minCol; col <= maxCol; col++)
            {
                yield return new Tile { Row = Depth, Col = col };
            }
        }

        internal Row Next()
        {
            return new Row { Depth = Depth + 1, StartSlope = StartSlope, EndSlope = EndSlope };
        }
    }
}
