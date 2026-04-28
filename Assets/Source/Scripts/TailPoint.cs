using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class TailPoint : CatPoint
    {
        [SerializeField] private Cat _cat;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private CatSnakePathVisual _pathVisual;

        private Coroutine _coroutine;
        private Vector3 _pendingTarget;
        public float SpeedMultiplier;
        private bool _catchUp;
        private Coroutine _movingCoroutine;
        private int _index;

        private void Start()
        {
            SpeedMultiplier = 1f;
            StartCoroutine(Moving());
        }


        public void Move()
        {
            if (_catchUp)
                return;

            if (_movingCoroutine != null)
                StopCoroutine(_movingCoroutine);

            _movingCoroutine = StartCoroutine(Moving());
        }

        private IEnumerator Moving()
        {
            while (true)
            {

                var currentLength = _cat.CatPath.GetNextPointIndex(this);
                var next = _cat.CatPath.GetPointAt(currentLength);

                if (currentLength > _cat.Length)
                {
                    SpeedMultiplier = 2f;
                }
                else if (currentLength < _cat.Length)
                {
                    next = null;
                }
                else
                {
                    SpeedMultiplier = 1f;
                }

                if (next == null)
                    yield return null;


                if (next != null)
                {
                    var dir = next.transform.position - transform.position;
                    transform.rotation = ToQuaternion(dir.normalized);
                    yield return MoveTo(next.transform.position);
                }
            }
        }

        private IEnumerator MoveTo(Vector3 gridPos)
        {
            var lerp = 0f;
            var elapsedTime = 0f;
            var startPos = transform.position;

            while (lerp < 1f)
            {
                elapsedTime += Time.fixedDeltaTime * SpeedMultiplier;
                elapsedTime = MathF.Round(elapsedTime, 2);
                lerp = elapsedTime / _cat.TimeToCell;
                var pos = Vector3.Lerp(startPos, gridPos, lerp);
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
}