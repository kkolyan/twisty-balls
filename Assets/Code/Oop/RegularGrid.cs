using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Code.Oop
{
    public class RegularGrid<T>
    {
        private struct Node
        {
            internal T value;
            internal int nextNodeId;

            public override string ToString()
            {
                return $"{nameof(value)}: {value}, {nameof(nextNodeId)}: {nextNodeId}";
            }
        }

        public readonly Vector2 offset;
        public readonly Vector2 size;
        public readonly float step;

        private readonly int[] _nodeIdsGrid;
        private Node[] _nodes;
        private int _nodeCount;

        public readonly Vector2Int gridSize;

        private static T[] _searchTmp;

        public RegularGrid(Vector2 offset, Vector2 size, float step, int initialPoolSize)
        {
            this.offset = offset;
            this.size = size;
            this.step = step;
            gridSize.x = Mathf.CeilToInt(size.x / step);
            gridSize.y = Mathf.CeilToInt(size.y / step);

            _nodes = new Node[initialPoolSize];
            _nodeIdsGrid = new int[gridSize.x * gridSize.y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CellIndex(int x, int y)
        {
            if (x < 0 || x >= gridSize.x) return -1;
            if (y < 0 || y >= gridSize.y) return -1;
            return x + y * gridSize.x;
        }

        public void Add(Vector2 pos, in T value)
        {
            Vector2 cell = (pos - offset) / step;
            int cellIndex = CellIndex(Mathf.FloorToInt(cell.x), Mathf.FloorToInt(cell.y));
            if (cellIndex < 0)
            {
                return;
            }

            if (_nodeCount >= _nodes.Length)
            {
                Array.Resize(ref _nodes, _nodes.Length * 2);
            }

            int prevNodeId = _nodeIdsGrid[cellIndex];
            int nodeId = ++_nodeCount;
            _nodes[nodeId - 1] = new Node
            {
                value = value,
                nextNodeId = prevNodeId
            };
            _nodeIdsGrid[cellIndex] = nodeId;
        }

        public void Clear()
        {
            Array.Clear(_nodeIdsGrid, 0, _nodeIdsGrid.Length);
            Array.Clear(_nodes, 0, _nodeCount);
            _nodeCount = 0;
        }

        public IEnumerable<T> Search(Vector2 pos)
        {
            if (_searchTmp == null)
            {
                _searchTmp = new T[1024 * 1024];
            }

            int found = SearchNonAlloc(pos, _searchTmp);
            for (int i = 0; i < found; i++)
            {
                yield return _searchTmp[i];
            }
        }

        public int SearchNonAlloc(Vector2 pos, T[] results)
        {
            int resultsCount = 0;

            Vector2 cell = (pos - offset) / step;
            SearchNode(ref resultsCount, results, CellIndex(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y)));
            SearchNode(ref resultsCount, results, CellIndex(Mathf.RoundToInt(cell.x - 1), Mathf.RoundToInt(cell.y)));
            SearchNode(ref resultsCount, results, CellIndex(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y - 1)));
            SearchNode(ref resultsCount, results, CellIndex(Mathf.RoundToInt(cell.x - 1), Mathf.RoundToInt(cell.y - 1)));

            return resultsCount;
        }

        private void SearchNode(ref int resultsCount, T[] results, int cellIndex)
        {
            if (cellIndex < 0)
            {
                return;
            }

            int nextNodeId = _nodeIdsGrid[cellIndex];
            while (nextNodeId > 0 && resultsCount < results.Length)
            {
                ref Node node = ref _nodes[nextNodeId - 1];
                results[resultsCount++] = node.value;
                nextNodeId = node.nextNodeId;
            }
        }
    }
}