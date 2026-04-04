using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.PlayerLoop; // Удалена - не используется

public class Cat : CatPoint
{
    [SerializeField] private GameGrid _grid;
    [SerializeField] private float _speed;
    [SerializeField] private CatSnakePathVisual _pathVisual;
    [SerializeField] private CatPoint _headPoint;
    [SerializeField] private TailPoint _tailPoint;
    [SerializeField] private Rigidbody2D _rb;

    private int _catLength;
    private PlayerInput _input;
    private float _timeToCell => _grid.CellSize / _speed;
    private Vector2 _lastDir;
    private Queue<CatPoint> _pathQueue;
    private Queue<Vector2> _dirs;

    public float TimeToCell => _timeToCell;

    private void Awake()
    {
        _catLength = 1;
        Application.targetFrameRate = 144;
        _pathQueue = new Queue<CatPoint>();
        _dirs = new Queue<Vector2>();
        _input = new PlayerInput();
        transform.position = _grid.GetGridPosition(transform.position);
        _tailPoint.transform.position = transform.position;
        StartCoroutine(Moving());
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input.Disable();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _catLength++;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _catLength--;
        }

        if (!_input.Player.Move.WasPressedThisFrame())
            return;

        var dir = _input.Player.Move.ReadValue<Vector2>();
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
                yield return MoveTo(nextCellPos);
                if (_pathQueue.Count >= _catLength)
                {
                    var old = _pathQueue.Dequeue();
                    var gridPos = _grid.GetGridPosition(_headPoint.transform.position);
                    if(_pathQueue.Count > 0)
                    {
                        gridPos = _grid.GetGridPosition(_pathQueue.Peek().transform.position);
                    }
                    _tailPoint.MovingTo(gridPos);
                    old.Destroy();
                }

                var pathGO = new GameObject();
                var point = pathGO.AddComponent<CatPoint>();
                pathGO.transform.position = nextCellPos;
                _pathQueue.Enqueue(point);
                
                _pathVisual.SetPath(_tailPoint, _pathQueue, _headPoint);
            }
            else
            {
                yield return null;
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
            elapsedTime += Time.fixedDeltaTime;
            elapsedTime = MathF.Round(elapsedTime, 2);
            lerp = elapsedTime / _timeToCell;
            var pos = Vector3.Lerp(startPos, gridPos, lerp);
            _rb.MovePosition(pos);
            yield return new WaitForFixedUpdate();
        }
    }

    private static Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angleDir - 90f);
    }
}
