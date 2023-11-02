using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLotJam.Pathfind
{
    public class Matrix
    {
        public Node[,] nodes;
        public int matrixSizeX, matrixSizeY;

        /**
        * Create a new grid with tile prices.
        * width: grid width.
        * height: grid height.
        * tiles_costs: 2d array of floats, representing the cost of every tile.
        *               0.0f = unwalkable tile.
        *               1.0f = normal tile.
        */
        public Matrix(int width, int height, float[,] tiles_costs)
        {
            matrixSizeX = width;
            matrixSizeY = height;
            nodes = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(tiles_costs[x, y], x, y);

                }
            }
        }

        /**
        * Create a new grid of just walkable / unwalkable.
        * width: grid width.
        * height: grid height.
        * walkable_tiles: the tilemap. true for walkable, false for blocking.
        */
        public Matrix(int width, int height, bool[,] walkable_tiles)
        {
            matrixSizeX = width;
            matrixSizeY = height;
            nodes = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
                }
            }
        }

        public Matrix(int width, int height)
        {
            matrixSizeX = width;
            matrixSizeY = height;
            nodes = new Node[width, height];

            bool[,] walkable_tiles = new bool[width,height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    walkable_tiles[x,y] = true;
                    nodes[x, y] = new Node(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < matrixSizeX && checkY >= 0 && checkY < matrixSizeY)
                    {
                       if(Mathf.Abs(x) != Mathf.Abs(y)) 
                           neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
    }
}
