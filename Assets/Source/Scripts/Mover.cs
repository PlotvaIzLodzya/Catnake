using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class Mover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private GameGrid _grid;
    [SerializeField] private Vector2 _dir;
    [SerializeField] private float _timeToCell;
    [SerializeField] private float _speed;

    [Header("Visuals")]
    [SerializeField] private LineRenderer _lineRenderer;

    private Vector2 _lastDir = Vector2.zero; // Для отслеживания смены направления

    private PlayerInput _input;

    void Start()
    {
        _timeToCell = 1f / _speed;
        _input = new();
        _input.Enable();
        // Настройка LineRenderer
        if (_lineRenderer == null)
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = false;
        _lineRenderer.positionCount = 2; // Минимум 2 точки для линии (Голова + Хвост)

        // Инициализация позиций
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);

        StartCoroutine(Moving());
    }

    void Update()
    {

        if (_input.Player.Move.WasPressedThisFrame())
        {
            Debug.Log("hi");
            _dir = _input.Player.Move.ReadValue<Vector2>();
        }
        // Требование 2: Первая точка всегда двигается с трансформом
        if (_lineRenderer != null)
        {
            _lineRenderer.SetPosition(_lineRenderer.positionCount, transform.position);
        }
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            var nextCellPos = _grid.GetCellInDirection(transform.position, _dir);
            transform.rotation = ToQuaternion(_dir);

            // Требование 1: Точки создаются во время смены направления
            // Проверяем, что мы движемся и направление отличается от последнего зафиксированного
            if (_dir.sqrMagnitude > 0f && _dir != _lastDir)
            {
                AddTrailPoint(transform.position);
                _lastDir = _dir;
            }

            if (_dir.sqrMagnitude > 0f)
                yield return MoveTo(nextCellPos);
            else
                yield return null;
        }
    }

    private IEnumerator MoveTo(Vector3 gridPos)
    {
        var dist = Vector2.Distance(gridPos, transform.position);
        var lerp = 0f;
        var elapsedTime = 0f;
        var startPos = transform.position;

        while (lerp < 1f)
        {
            elapsedTime += Time.deltaTime;
            lerp = elapsedTime / _timeToCell;
            transform.position = Vector3.Lerp(startPos, gridPos, lerp);
            yield return null;
        }

        // Фиксируем позицию в конце движения, чтобы линия не дергалась
        transform.position = gridPos;
    }

    // Helper для добавления точек в LineRenderer
    private void AddTrailPoint(Vector3 pos)
    {
        int currentCount = _lineRenderer.positionCount;
        _lineRenderer.positionCount = currentCount + 1;
        // Новая точка добавляется в конец (индекс currentCount)
        _lineRenderer.SetPosition(currentCount, pos);
    }

    // Заглушка для примера, у вас должна быть своя реализация
    private Quaternion ToQuaternion(Vector2 direction)
    {
        if (direction == Vector2.zero) return transform.rotation;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }
}
