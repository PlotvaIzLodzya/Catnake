using System;
using System.Collections;
using UnityEngine;

public class TailPoint : CatPoint
{
    [SerializeField] private Cat _cat;
    [SerializeField] private Rigidbody2D _rb;

    private Coroutine _coroutine;
    private Vector3 _pendingTarget;

    public void MovingTo(Vector3 pos)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _rb.MovePosition(_pendingTarget);
        }

        _coroutine = StartCoroutine(MoveTo(pos));
    }

    private IEnumerator MoveTo(Vector3 gridPos)
    {
        _pendingTarget = gridPos;
        var lerp = 0f;
        var elapsedTime = 0f;
        var startPos = transform.position;
        var startRot = transform.rotation;
        var diff = gridPos - startPos;
        var targetRot = diff.sqrMagnitude > 0.0001f ? ToQuaternion(diff) : startRot;

        while (lerp < 1f)
        {
            elapsedTime += Time.fixedDeltaTime;
            lerp = elapsedTime / _cat.TimeToCell;
            lerp = MathF.Round(lerp, 2);
            var pos = Vector3.Lerp(startPos, gridPos, lerp);
            _rb.MovePosition(pos);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, lerp);
            yield return new WaitForFixedUpdate();
        }
    }

    private Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(0, 0, angleDir - 180f);
    }
}
