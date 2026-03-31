using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SplineContainer))]
public class SplineLineRenderer : MonoBehaviour
{
    [Header("Настройки отрисовки")]
    [Range(2, 1000)]
    public int resolution = 100;

    [Tooltip("Отрисовывать все сплайны в контейнере")]
    public bool drawAllSplines = false;

    [Tooltip("Автоматически обновлять при изменении сплайна")]
    public bool autoUpdate = true;

    [Header("Настройки LineRenderer")]
    public float lineWidth = 0.1f;

    private LineRenderer lineRenderer;
    private SplineContainer splineContainer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splineContainer = GetComponent<SplineContainer>();

        SetupLineRenderer();
    }

    void Start()
    {
        UpdateSpline();
    }

    void LateUpdate()
    {
        if (autoUpdate)
        {
            UpdateSpline();
        }
    }

    void SetupLineRenderer()
    {
        // КРИТИЧЕСКИ ВАЖНО: useWorldSpace = true для мировых координат
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = false;
        lineRenderer.alignment = LineAlignment.View;

        //// Настройка материала
        //if (lineMaterial != null)
        //{
        //    lineRenderer.material = lineMaterial;
        //}
        //else if (lineRenderer.material == null)
        //{
        //    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //}

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void UpdateSpline()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0 || splineContainer.Spline.Count == 0)
            return;

        int splineCount = drawAllSplines ? splineContainer.Splines.Count : 1;
        int totalPoints = 0;

        // Сначала считаем общее количество точек
        for (int s = 0; s < splineCount; s++)
        {
            var spline = splineContainer.Splines[s];
            totalPoints += spline.Closed ? resolution : resolution + 1;
        }

        lineRenderer.positionCount = totalPoints;

        // Заполняем позиции
        int currentIndex = 0;

        for (int s = 0; s < splineCount; s++)
        {
            var spline = splineContainer.Splines[s];
            int pointsForSpline = spline.Closed ? resolution : resolution + 1;

            for (int i = 0; i < pointsForSpline; i++)
            {
                float t = i / (float)resolution;

                // Получаем позицию из сплайна (локальные координаты сплайна)
                Vector3 localPosition = spline.EvaluatePosition(t);

                // КРИТИЧЕСКИ ВАЖНО: Конвертируем в мировые координаты
                Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);

                lineRenderer.SetPosition(currentIndex, worldPosition);
                currentIndex++;
            }
        }
    }

    // Получение точки в мировых координатах
    public Vector3 GetPointOnSpline(float t, int splineIndex = 0)
    {
        if (splineContainer == null || splineContainer.Splines.Count <= splineIndex)
            return Vector3.zero;

        Vector3 localPos = splineContainer.Splines[splineIndex].EvaluatePosition(Mathf.Clamp01(t));
        return splineContainer.transform.TransformPoint(localPos);
    }

    // Получение касательной в мировых координатах
    public Vector3 GetTangentOnSpline(float t, int splineIndex = 0)
    {
        if (splineContainer == null || splineContainer.Splines.Count <= splineIndex)
            return Vector3.forward;

        Vector3 localTangent = splineContainer.Splines[splineIndex].EvaluateTangent(Mathf.Clamp01(t));
        return splineContainer.transform.TransformDirection(localTangent);
    }

    [ContextMenu("Force Update Spline")]
    public void ForceUpdate()
    {
        UpdateSpline();
    }

    //private void OnDrawGizmos()
    //{
    //    if (splineContainer == null || splineContainer.Splines.Count == 0)
    //        return;

    //    int splineCount = drawAllSplines ? splineContainer.Splines.Count : 1;

    //    for (int s = 0; s < splineCount; s++)
    //    {
    //        var spline = splineContainer.Splines[s];

    //        Gizmos.color = Color.yellow;
    //        foreach (var knot in spline)
    //        {
    //            Vector3 worldPos = splineContainer.transform.TransformPoint(knot.Position);
    //            Gizmos.DrawSphere(worldPos, 0.1f);
    //        }

    //        Gizmos.color = Color.green;
    //        int points = spline.Closed ? resolution : resolution + 1;
    //        Vector3 previousPoint = splineContainer.transform.TransformPoint(spline.EvaluatePosition(0));

    //        for (int i = 1; i <= points; i++)
    //        {
    //            float t = i / (float)resolution;
    //            Vector3 localPos = spline.EvaluatePosition(t);
    //            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);

    //            Gizmos.DrawLine(previousPoint, worldPos);
    //            previousPoint = worldPos;
    //        }
    //    }
    //}
}