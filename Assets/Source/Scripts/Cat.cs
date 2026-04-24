using Assets.Source.Scripts.LevelFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class Cat : CatPoint, ICat
    {
        [SerializeField] private GameGrid _grid;
        [SerializeField] private float _speed;
        [SerializeField] private CatSnakePathVisual _pathVisual;
        [SerializeField] private CatPoint _headPoint;
        [SerializeField] private TailPoint _tailPoint;
        [SerializeField] private Rigidbody2D _rb;

        private PlayerInput _input;
        private Vector2 _lastDir;
        private List<CatPoint> _pathPoints;
        private Queue<Vector2> _dirs;
        private Coroutine _movingCoroutine;

        public int Length { get; private set; }
        public CatPath CatPath { get; private set; }

        public float TimeToCell => _grid.CellSize / _speed;
        private bool _damageTaken;

        private void Awake()
        {
            Length = 1;
            Application.targetFrameRate = 144;
            _pathPoints = new();
            _dirs = new Queue<Vector2>();
            _input = new PlayerInput();
            transform.position = _grid.GetGridPosition(transform.position);
            _tailPoint.transform.position = transform.position;
            _movingCoroutine = StartCoroutine(Moving());
            CatPath = new(_pathPoints, this);
        }

        private void OnEnable() => _input.Enable();

        private void OnDisable() => _input.Disable();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Length++;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Length--;
            }

            var index = CatPath.GetNextPointIndex(_tailPoint);
            var body = CatPath.GetPoints(index);

            foreach (var catPathPoint in _pathPoints)
            {
                _grid.FreeCell(catPathPoint.transform.position);
            }

            foreach (var point in body)
            {
                _grid.Ocupy(point.transform.position);
            }

            _pathVisual.SetPath(_tailPoint, body, _headPoint);

            if (!_input.Player.Move.WasPressedThisFrame())
                return;

            var dir = _input.Player.Move.ReadValue<Vector2>();

            if (dir.x != 0 && dir.y != 0)
                dir = Vector2.zero;

            if (_dirs.Count > 2)
                return;

            if (_dirs.Count > 0)
            {
                if (Vector2.Angle(_dirs.Peek(), dir) < 95f)
                    _dirs.Enqueue(dir);
            }
            else if (Vector2.Angle(transform.up, dir) < 95f)
            {
                _dirs.Enqueue(dir);
            }
        }

        public void Feed(float value)
        {
            var length = Mathf.RoundToInt(value);

            Length += length;
        }

        public void TakeDamage(float damage)
        {
            var length = Mathf.RoundToInt(damage);
            _damageTaken = true;
            StartCoroutine(OnDamaged());
            Length -= length;
        }

        private IEnumerator Moving()
        {
            while (true)
            {
                if (_dirs.Count == 0 && _lastDir.sqrMagnitude == 0)
                {
                    yield return null;
                    continue;
                }

                Vector2 dir = _dirs.Count > 0 ? _dirs.Dequeue() : _lastDir;
                var nextCellPos = _grid.GetCellInDirection(transform.position, dir);

                if (dir.sqrMagnitude > 0f)
                    _lastDir = dir;

                transform.rotation = ToQuaternion(dir);

                if (_lastDir.sqrMagnitude > 0f)
                {
                    
                    yield return MoveTo(nextCellPos, TimeToCell);
                    var pathGO = new GameObject();
                    var point = pathGO.AddComponent<CatPoint>();
                    if(_pathPoints.Count > 1)
                    {
                        var pointBehindHead = CatPath.GetPointAt(0);
                        var col = pointBehindHead.AddComponent<BoxCollider2D>();
                        col.isTrigger = true;
                        col.size = Vector2.one * 0.3f;

                        for (int i = _pathPoints.Count - 1; i > Length; i--)
                        {
                            var p = CatPath.GetPointAt(i);
                            p.DisableCollision();
                        }
                    }    
                    _pathPoints.Add(point);
        
                    if (_pathPoints.Count > Length + 5)
                    {
                        var catPoint = _pathPoints[0];
                        catPoint.Destroy();
                        _pathPoints.RemoveAt(0);
                    }
                    for (int i = 0; i < _pathPoints.Count; i++)
                    {
                        CatPoint p = _pathPoints[i];
                        p.name = $"Point: {i}";
                    }
                    pathGO.transform.position = nextCellPos;

                }
                else
                {
                    yield return null;
                }
            }
        }

        private IEnumerator MoveTo(Vector3 gridPos, float time, bool eased = false)
        {
            var lerp = 0f;
            var elapsedTime = 0f;
            var startPos = transform.position;
            while (lerp < 1f)
            {
                elapsedTime += Time.fixedDeltaTime;
                elapsedTime = MathF.Round(elapsedTime, 2);
                lerp = elapsedTime / time;
                if (eased)
                    lerp = EaseOutBack(lerp);
                var pos = Vector3.Lerp(startPos, gridPos, lerp);


                _rb.MovePosition(pos);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator OnDamaged()
        {
            var previousPos = CatPath.GetPointAt(0).transform.position;
            StopCoroutine(_movingCoroutine);
            yield return MoveTo(previousPos, 0.3f, true);
            if(_pathPoints.Count > 1)
            {
                for (int i = _pathPoints.Count - 1; i > Length; i--)
                {
                    var p = CatPath.GetPointAt(i);
                    p.DisableCollision();
                }

            }
            _damageTaken = false;

            yield return new WaitForSeconds(0.2f);
            _movingCoroutine = StartCoroutine(Moving());
        }

        private static Quaternion ToQuaternion(Vector2 dir)
        {
            float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, angleDir - 90f);
        }

        float EaseOutBack(float lerp)
        {
            var c1 = 1.70158f;
            var c3 = c1 + 1f;

            return 1f + c3* Mathf.Pow(lerp - 1f, 3f) + c1* Mathf.Pow(lerp - 1f, 2f);
        }
}
}
