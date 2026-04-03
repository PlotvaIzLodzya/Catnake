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
            var t = Mathf.Clamp01(lerp);
            _rb.MovePosition(Vector3.Lerp(startPos, gridPos, t));
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return new WaitForFixedUpdate();
        }
    }

    private Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(0, 0, angleDir - 180f);
    }
}
