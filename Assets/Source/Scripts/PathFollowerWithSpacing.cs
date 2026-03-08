using UnityEngine;
using System.Collections.Generic;

public class PathFollowerWithSpacing : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [Header("Настройки")]
    [Tooltip("Количество точек в цепочке")]
    public int pointCount = 20;

    [Tooltip("Расстояние между точками вдоль пути")]
    public float spacing = 0.5f;

    [Tooltip("Размер точек")]
    public float pointSize = 0.15f;

    [Tooltip("Цвет точек")]
    public Color pointColor = Color.cyan;

    [Tooltip("Максимальная длина хранимого пути (для оптимизации)")]
    public float maxPathLength = 100f;

    [Header("Ссылки")]
    [Tooltip("Главная точка, чей путь повторяем")]
    public Transform leader;

    private List<Transform> points = new List<Transform>();
    private List<Vector3> pathHistory = new List<Vector3>();
    private List<float> pathDistances = new List<float>(); // Накопленное расстояние для каждой позиции
    private float totalPathLength = 0f;

    void Start()
    {

        // Создаём точки-последователи
        for (int i = 0; i < pointCount; i++)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = $"PathPoint_{i}";
            point.transform.position = leader.position;
            point.transform.localScale = Vector3.one * pointSize;

            // Убираем коллайдер
            Collider collider = point.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            // Настраиваем материал
            Renderer renderer = point.GetComponent<Renderer>();
            if (renderer != null)
            {
                float alpha = 1f - (float)i / pointCount;
                renderer.material.color = new Color(pointColor.r, pointColor.g, pointColor.b, alpha);
            }

            points.Add(point.transform);
        }

        // Инициализируем историю
        pathHistory.Add(leader.position);
        pathDistances.Add(0f);
    }

    void Update()
    {
        if (leader == null) return;

        // Добавляем новую позицию лидера в историю
        Vector3 currentPos = leader.position;
        Vector3 lastPos = pathHistory[pathHistory.Count - 1];
        float segmentLength = Vector3.Distance(currentPos, lastPos);

        // Добавляем позицию только если лидер двигается
        if (segmentLength > 0.001f)
        {
            totalPathLength += segmentLength;
            pathHistory.Add(currentPos);
            pathDistances.Add(totalPathLength);

            // Очищаем старую историю для оптимизации
            CleanupOldPath();
        }

        // Размещаем точки на заданном расстоянии вдоль пути
        _line.positionCount = pointCount;
        for (int i = 0; i < points.Count; i++)
        {
            float targetDistance = totalPathLength - (i + 1) * spacing;

            if (targetDistance < 0)
            {
                // Точка ещё не начала движение (путь слишком короткий)
                points[i].position = pathHistory[0];
                continue;
            }

            // Находим позицию в истории на нужном расстоянии
            Vector3 pointPosition = GetPositionAtDistance(targetDistance);
            points[i].position = pointPosition;
            _line.SetPosition(i, pointPosition);
        }
    }

    // Находит позицию вдоль пути на заданном накопленном расстоянии
    Vector3 GetPositionAtDistance(float targetDistance)
    {
        if (pathHistory.Count < 2) return leader.position;

        // Ищем сегмент пути, где находится нужное расстояние
        for (int i = pathDistances.Count - 1; i > 0; i--)
        {
            if (pathDistances[i] >= targetDistance && pathDistances[i - 1] <= targetDistance)
            {
                // Интерполируем между двумя точками истории
                float segmentStart = pathDistances[i - 1];
                float segmentEnd = pathDistances[i];
                float segmentLength = segmentEnd - segmentStart;

                if (segmentLength > 0.001f)
                {
                    float t = (targetDistance - segmentStart) / segmentLength;
                    t = Mathf.Clamp01(t);
                    return Vector3.Lerp(pathHistory[i - 1], pathHistory[i], t);
                }
            }
        }

        // Если не нашли, возвращаем первую позицию
        return pathHistory[0];
    }

    // Очищает старую историю пути для оптимизации памяти
    void CleanupOldPath()
    {
        float minLengthNeeded = totalPathLength - maxPathLength;

        while (pathDistances.Count > 2 && pathDistances[1] < minLengthNeeded)
        {
            pathHistory.RemoveAt(0);
            pathDistances.RemoveAt(0);

            // Корректируем накопленные расстояния
            float removedDistance = pathDistances[0];
            for (int i = 0; i < pathDistances.Count; i++)
            {
                pathDistances[i] -= removedDistance;
            }
            totalPathLength -= removedDistance;
        }
    }

    // Отрисовка в редакторе
    void OnDrawGizmos()
    {
        if (leader != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(leader.position, pointSize * 1.5f);
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
            {
                float alpha = 1f - (float)i / pointCount;
                Gizmos.color = new Color(pointColor.r, pointColor.g, pointColor.b, alpha);
                Gizmos.DrawSphere(points[i].position, pointSize);
            }
        }
    }
}
