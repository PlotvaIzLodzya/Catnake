using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[ExecuteAlways]
public class UnitySplineFromTransforms : MonoBehaviour
{
    [Header("Контрольные точки")]
    [Tooltip("Массив Transform объектов - через них пройдёт сплайн")]
    public Transform[] controlPoints;

    [Header("Настройки")]
    [Tooltip("Замыкать сплайн")]
    public bool loop = false;

    [Tooltip("Автоматически обновлять при перемещении точек")]
    public bool autoUpdate = true;

    private SplineContainer splineContainer;
    private Spline spline;
    private Vector3[] previousPositions;

    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        BuildSplineFromTransforms();
    }

    void LateUpdate()
    {
        if (autoUpdate)
        {
            CheckTransformsChanged();
        }
    }

    public void BuildSplineFromTransforms()
    {
        if (controlPoints == null || controlPoints.Length < 2)
        {
            Debug.LogWarning("Нужно минимум 2 контрольные точки!");
            return;
        }

        // Создаём новый сплайн
        spline = new Spline();
        spline.SetTangentMode(TangentMode.AutoSmooth);
        spline.Closed = loop;

        // Добавляем узлы только с позициями - касательные рассчитаются автоматически
        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (controlPoints[i] == null) continue;
            var tangentMode = TangentMode.AutoSmooth;
            if (i == 0 || i == controlPoints.Length - 1)
            {
                tangentMode = TangentMode.Linear;
            }
            Vector3 position = controlPoints[i].position;

            // ✅ Касательные не указываем - Unity рассчитает автоматически
            var knot = new BezierKnot(position, Vector3.zero, Vector3.zero);
            spline.Add(knot, tangentMode);
        }

        // ✅ Автоматический расчёт касательных через SplineUtility
        //SplineUtility.CalculateTangents(spline, SplineUtility.TangentMode.Continuous);

        // Применяем сплайн к контейнеру
        if (splineContainer.Splines.Count > 0)
        {
            splineContainer.Spline = spline;
        }
    }

    void CheckTransformsChanged()
    {
        if (controlPoints == null || controlPoints.Length == 0)
            return;

        if (previousPositions == null || previousPositions.Length != controlPoints.Length)
        {
            previousPositions = new Vector3[controlPoints.Length];
            BuildSplineFromTransforms();
            return;
        }

        bool changed = false;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (controlPoints[i] == null) continue;

            if (previousPositions[i] != controlPoints[i].position)
            {
                changed = true;
                previousPositions[i] = controlPoints[i].position;
            }
        }

        if (changed)
        {
            BuildSplineFromTransforms();
        }
    }

    // Получение точки на сплайне (мировые координаты)
    public Vector3 GetPointOnSpline(float t)
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            return Vector3.zero;

        return splineContainer.Splines[0].EvaluatePosition(Mathf.Clamp01(t));
    }

    // Получение касательной (мировые координаты)
    public Vector3 GetTangentOnSpline(float t)
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            return Vector3.forward;

        return splineContainer.Splines[0].EvaluateTangent(Mathf.Clamp01(t));
    }

    [ContextMenu("Rebuild Spline")]
    public void Rebuild()
    {
        BuildSplineFromTransforms();
    }

    //private void OnDrawGizmos()
    //{
    //    if (controlPoints == null || controlPoints.Length == 0)
    //        return;

    //    // Рисуем контрольные точки
    //    Gizmos.color = Color.yellow;
    //    foreach (var point in controlPoints)
    //    {
    //        if (point != null)
    //        {
    //            Gizmos.DrawSphere(point.position, 0.15f);
    //        }
    //    }

    //    // Рисуем соединения
    //    Gizmos.color = Color.gray;
    //    for (int i = 0; i < controlPoints.Length - 1; i++)
    //    {
    //        if (controlPoints[i] != null && controlPoints[i + 1] != null)
    //        {
    //            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
    //        }
    //    }

    //    if (loop && controlPoints.Length > 2)
    //    {
    //        Gizmos.DrawLine(
    //            controlPoints[0].position,
    //            controlPoints[controlPoints.Length - 1].position
    //        );
    //    }
    //}
}