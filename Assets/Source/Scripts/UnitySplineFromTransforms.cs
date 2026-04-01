using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[ExecuteAlways]
public class UnitySplineFromTransforms : MonoBehaviour
{
    [Header("Контрольные точки")]
    [Tooltip("Массив Transform объектов - через них пройдёт сплайн")]
    public Transform[] controlPoints = new Transform[0];

    [Header("Настройки")]
    [Tooltip("Замыкать сплайн")]
    public bool loop = false;

    [Tooltip("Автоматически обновлять при перемещении точек")]
    public bool autoUpdate = true;

    private SplineContainer splineContainer;
    private Spline _spline;
    private Vector3[] previousPositions;

    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        BuildSplineFromTransforms();
    }

    void LateUpdate()
    {
        if (autoUpdate)
            CheckTransformsChanged();
    }

    /// <summary>
    /// Задаёт контрольные точки из змейки и синхронизирует кэш позиций, чтобы не пересобирать сплайн дважды в кадр.
    /// </summary>
    public void SetControlPointsFromCatPoints(IReadOnlyList<CatPoint> points)
    {
        int n = points.Count;
        if (controlPoints.Length != n)
            controlPoints = new Transform[n];

        for (int i = 0; i < n; i++)
            controlPoints[i] = points[i].transform;

        BuildSplineFromTransforms();
        SyncPreviousPositionsFromTransforms();
    }

    public void BuildSplineFromTransforms()
    {
        if (controlPoints.Length < 2)
            return;

        if (_spline == null)
            _spline = new Spline();
        else
            _spline.Clear();

        _spline.SetTangentMode(TangentMode.AutoSmooth);
        _spline.Closed = loop;

        for (int i = 0; i < controlPoints.Length; i++)
        {
            var tangentMode = TangentMode.AutoSmooth;
            if (i == 0 || i == controlPoints.Length - 1)
                tangentMode = TangentMode.Linear;

            Vector3 position = controlPoints[i].position;
            var knot = new BezierKnot(position, Vector3.zero, Vector3.zero);
            _spline.Add(knot, tangentMode);
        }

        if (splineContainer.Splines.Count > 0)
            splineContainer.Spline = _spline;
    }

    void CheckTransformsChanged()
    {
        if (controlPoints.Length == 0)
            return;

        if (previousPositions == null || previousPositions.Length != controlPoints.Length)
        {
            previousPositions = new Vector3[controlPoints.Length];
            BuildSplineFromTransforms();
            SyncPreviousPositionsFromTransforms();
            return;
        }

        bool changed = false;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Vector3 pos = controlPoints[i].position;
            if (previousPositions[i] != pos)
            {
                changed = true;
                previousPositions[i] = pos;
            }
        }

        if (changed)
            BuildSplineFromTransforms();
    }

    private void SyncPreviousPositionsFromTransforms()
    {
        if (previousPositions == null || previousPositions.Length != controlPoints.Length)
            previousPositions = new Vector3[controlPoints.Length];

        for (int i = 0; i < controlPoints.Length; i++)
            previousPositions[i] = controlPoints[i].position;
    }

    public Vector3 GetPointOnSpline(float t)
    {
        if (splineContainer.Splines.Count == 0)
            return Vector3.zero;

        return splineContainer.Splines[0].EvaluatePosition(Mathf.Clamp01(t));
    }

    public Vector3 GetTangentOnSpline(float t)
    {
        if (splineContainer.Splines.Count == 0)
            return Vector3.forward;

        return splineContainer.Splines[0].EvaluateTangent(Mathf.Clamp01(t));
    }

    [ContextMenu("Rebuild Spline")]
    public void Rebuild()
    {
        BuildSplineFromTransforms();
    }
}
