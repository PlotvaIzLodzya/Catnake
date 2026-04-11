using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{

    /// <summary>
    /// Порядок точек: хвост → сегменты тела → голова. Обновляет сплайн; <see cref="SplineLineRenderer"/> на том же объекте с <see cref="SplineContainer"/> строит LineRenderer по сплайну.
    /// </summary>
    public class CatSnakePathVisual : MonoBehaviour
    {
        [SerializeField] private UnitySplineFromTransforms _spline;
        [SerializeField] private SplineLineRenderer _splineLine;

        private readonly List<CatPoint> _orderedPoints = new(32);

        public void SetPath(TailPoint tail, List<CatPoint> body, CatPoint head)
        {
            _orderedPoints.Clear();
            _orderedPoints.Add(tail);
            foreach (var p in body)
                _orderedPoints.Add(p);
            _orderedPoints.Add(head);

            _spline.SetControlPointsFromCatPoints(_orderedPoints);
            _splineLine.UpdateSpline();
        }
    }
}