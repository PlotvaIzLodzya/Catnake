using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Source.Scripts.LevelFeatures
{
    public class GameGrid : MonoBehaviour
    {
        [SerializeField] private float _cellSize;
        [SerializeField] private Transform _leftDown;
        [SerializeField] private Transform _rightTop;

        private List<Vector2> _allCells;
        private HashSet<Vector2> _occupiedCells;

        private float _halfCellSize => _cellSize * 0.5f;
        public float HalfCellSize => _halfCellSize;
        public float CellSize => _cellSize;

        private void Awake()
        {
            _allCells = new();
            _occupiedCells = new();
            var gridLeftDown = GetGridPosition(_leftDown.position);
            var gridRightTop = GetGridPosition(_rightTop.position);
            for (float x = gridLeftDown.x; x < gridRightTop.x; x+= _cellSize)
            {
                for (float y = gridLeftDown.y; y < gridRightTop.y; y+= _cellSize)
                {
                    _allCells.Add(new Vector2(x, y));
                }
            }
        }

        public Vector2 GetRandomGridPos()
        {
            var randomIndex = Random.Range(0, _allCells.Count);
            var randomCell = _allCells[randomIndex];
            
            var gridPos = GetGridPosition(randomCell);

            return gridPos;
        }

        public Vector2 GetRandomFreeGridPos()
        {
            var freeCels = _allCells.Except(_occupiedCells).ToList();
            var randomIndex = Random.Range(0, freeCels.Count);
            var randomCell = freeCels[randomIndex];

            return randomCell;
        }

        public void Ocupy(Vector2 worldPos)
        {
            var gridPos = GetGridPosition(worldPos);
            _occupiedCells.Add(gridPos);
        }

        public void FreeCell(Vector2 worldPos)
        {
            var gridPos = GetGridPosition(worldPos);
            _occupiedCells.Remove(gridPos);
        }


        public bool IsInCell(Vector3 worldPos)
        {
            var dist = DistToCellCenter(worldPos);
            return dist <= _cellSize * 0.5f;
        }

        public float DistToCellCenter(Vector3 worldPos)
        {
            var gridPos = GetGridPosition(worldPos);
            var dist = Vector2.Distance(gridPos, worldPos);

            return dist;
        }

        public float PathToCellElapsedNormalized(Vector3 worldPos)
        {
            var dist = DistToCellCenter(worldPos);
            var lerp = 1 - (dist / _halfCellSize);

            return lerp;
        }

        public bool IsOnCellEnter(Vector3 worldPos, Vector2 dir)
        {
            var gridPos = GetGridPosition(worldPos);
            var cellDir = (gridPos - worldPos).normalized;
            var delta = worldPos - gridPos;
            var angle = Vector2.Angle(cellDir, dir);

            return angle < 90f;
        }

        public Vector3 GetCellInDirection(Vector3 worldPos, Vector3 direction)
        {
            var gridPos = GetGridPosition(worldPos);
            var nextGridPos = direction.normalized * _cellSize;


            return gridPos + nextGridPos;

        }

        /// <summary>
        /// Преобразует мировые координаты в позицию центра ячейки сетки.
        /// </summary>
        /// <param name="worldPosition">Входные мировые координаты.</param>
        /// <returns>Координаты центра ячейки сетки.</returns>
        public Vector3 GetGridPosition(Vector3 worldPosition)
        {
            // Определяем индексы ячейки по горизонтали и вертикали
            int cellX = Mathf.FloorToInt(worldPosition.x / _cellSize);
            int cellY = Mathf.FloorToInt(worldPosition.y / _cellSize);

            // Вычисляем центр ячейки
            float centerX = (cellX + 0.5f) * _cellSize;
            float centerY = (cellY + 0.5f) * _cellSize;

            return new Vector3(centerX, centerY, worldPosition.z);
        }
    }
}