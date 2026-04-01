using System.Collections;
using UnityEngine;

public class TailPoint : CatPoint
{
    [SerializeField] private Cat _cat;
    [SerializeField] private Rigidbody2D _rb;

    private Coroutine _coroutine;

    public void MovingTo(Vector3 pos)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(MoveTo(pos));
    }

    private IEnumerator MoveTo(Vector3 gridPos)
    {
        var lerp = 0f;
        var elapsedTime = 0f;
        var startPos = transform.position;
        var startRot = transform.rotation;
        while (lerp < 1f)
        {
            elapsedTime += Time.deltaTime;
            lerp = elapsedTime / _cat.TimeToCell;
            var pos = Vector3.LerpUnclamped(startPos, gridPos, lerp);
            var dir = gridPos - transform.position;
            var rot = ToQuaternion(dir);
            transform.rotation = Quaternion.Lerp(startRot, rot, lerp * 10);
            //pos = _grid.GetGridPosition(pos);
            _rb.MovePosition(pos);
            yield return new WaitForFixedUpdate();
        }
    }

    private Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(0, 0, angleDir - 180f);
    }
}
