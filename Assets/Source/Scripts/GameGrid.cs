using UnityEditor;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private float _cellSize;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Vector2Int _gridSize;

    private float _halfCellSize => _cellSize * 0.5f;
    public float HalfCellSize => _halfCellSize;
    public float CellSize => _cellSize;

    [ContextMenu(nameof(GenerateGrid))]
    private void GenerateGrid()
    {
        PlaceCells(_cellSize, _gridSize);
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

    /// <summary>
    /// Создаёт и выкладывает клетки в виде сетки.
    /// </param>
    /// <param name="cellSize">Размер одной клетки в мировых единицах (ширина, высота)</param>
    /// <param name="gridSize">Количество клеток по горизонтали и вертикали</param>
    public void PlaceCells(float cellSize, Vector2 gridSize)
    {
        if (_cellPrefab == null)
        {
            Debug.LogError("Cell prefab is not assigned!");
            return;
        }

        // Очищаем предыдущие клетки (если нужно перестроить сетку)
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Рассчитываем смещение, чтобы сетка была центрирована относительно родительского объекта
        float totalWidth = cellSize * gridSize.x;
        float totalHeight = cellSize * gridSize.y;
        Vector2 startPosition = new Vector2(
            -totalWidth / 2f + cellSize / 2f,
            -totalHeight / 2f + cellSize / 2f
        );

        // Проходим по всем ячейкам сетки
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                // Позиция текущей клетки
                Vector2 cellPosition = startPosition + new Vector2(x * cellSize, y * cellSize);

                // Создаём экземпляр префаба
                GameObject cell = PrefabUtility.InstantiatePrefab(_cellPrefab, transform) as GameObject;
                cell.transform.localPosition = cellPosition;

                // Масштабируем клетку, если префаб имеет размер 1x1, чтобы он соответствовал cellSize
                // (если префаб уже имеет нужный размер, эту строку можно убрать)
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);

                // Дополнительно можно задать имя для удобства
                cell.name = $"Cell_{x}_{y}";
            }
        }
    }
}
