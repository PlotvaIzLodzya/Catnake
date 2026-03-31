using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<CatPoint> points;

    void LateUpdate()
    {
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
                lineRenderer.SetPosition(i, points[i].transform.position);
        }
    }


}
