using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class CatPath
    {
        private List<CatPoint> _pathPoints;
        private ICatLength _cat;

        public CatPath(List<CatPoint> pathPoints, ICatLength cat)
        {
            _pathPoints = pathPoints;
            _cat = cat;
        }

        public CatPoint GetPointAt(int targetLength)
        {
            targetLength = _pathPoints.Count - targetLength;
            targetLength = (int)Mathf.Clamp(targetLength, 0f, _pathPoints.Count);

            if (_pathPoints.Count > targetLength)
            {
                var catPoint = _pathPoints[targetLength];
                return catPoint;
            }

            return null;
        }

        public List<CatPoint> GetPoints(int targetLength)
        {
            var list = new List<CatPoint>();

            if (_pathPoints.Count > targetLength)
                list = _pathPoints.TakeLast(targetLength).ToList();

            return list;
        }

        public CatPoint GetNextPoint(CatPoint catPoint)
        {
            var index = GetNextPointIndex(catPoint);

            return GetPointAt(index);
        }

        public int GetNextPointIndex(CatPoint catPoint)
        {
            var target = _pathPoints.Count - _cat.Length;
            target -= 2;
            target = Mathf.Clamp(target, 1, target);
            for (int i = target; i < _pathPoints.Count; i++)
            {
                var current = _pathPoints[i];
                var previous = _pathPoints[i - 1];

                var distBetweenPoints = Vector2.Distance(current.transform.position, previous.transform.position);
                var distToCatPoint = Vector2.Distance(previous.transform.position, catPoint.transform.position);

                if (distToCatPoint < distBetweenPoints)
                {
                    var index = _pathPoints.Count - i;
                    var dist = Vector2.Distance(catPoint.transform.position, current.transform.position);
                    return index;
                }
            }

            return _cat.Length;
        }

        public List<CatPoint> GetPointsUntil(CatPoint catPoint)
        {
            var index = GetNextPointIndex(catPoint);

            return GetPoints(index);
        }

        public bool IsBetween(Transform a, Transform b, Transform c, float distanceTolerance = 0.01f)
        {
            // Используем Vector3 для корректной работы в 3D
            Vector3 ab = b.position - a.position;
            Vector3 ac = c.position - a.position;
            float abSqMag = ab.sqrMagnitude;
            float tTolerance = 0.01f; // Согласованный допуск для параметра t

            // Если A и B практически совпадают
            if (abSqMag < distanceTolerance * distanceTolerance)
            {
                return Vector3.Distance(a.position, c.position) < distanceTolerance;
            }

            // Проекция C на прямую AB: t = 0 → A, t = 1 → B
            float t = Vector3.Dot(ac, ab) / abSqMag;

            // Проверяем, что проекция попадает в отрезок [0, 1] с допуском
            if (t < -tTolerance || t > 1f + tTolerance)
            {
                return false;
            }

            // Ближайшая точка на отрезке AB к точке C
            Vector3 closestPoint = a.position + ab * Mathf.Clamp01(t);

            // Проверяем расстояние от C до отрезка
            return Vector3.Distance(c.position, closestPoint) < distanceTolerance;
        }
    }
}

