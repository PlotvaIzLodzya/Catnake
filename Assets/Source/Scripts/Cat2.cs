using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Cat2 : CatPoint
{
    [SerializeField] private GameGrid _grid;
    [SerializeField] private float _speed;
    [SerializeField] private Transform _end;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private TransformLineRenderer _lineRenderer;
    [SerializeField] private UnitySplineFromTransforms _spline;
    [SerializeField] private CatPoint _headPoint;
    [SerializeField] private TailPoint _tailPoint;

    private PlayerInput _input;
    private float _timeToCell => _grid.CellSize / _speed;
    private Vector2 _dir;
    private Vector2 _lastDir;
    public List<Vector3> PathPoints;
    public Queue<CatPoint> PathQueue;
    private Queue<Vector2> _dirs;

    public float TimeToCell => _timeToCell;

    private void Awake()
    {

        Application.targetFrameRate = 144;
        PathQueue = new();
        _dirs = new();
        _input = new();
        PathPoints = new();
        transform.position = _grid.GetGridPosition(transform.position);
        _tailPoint.transform.position = transform.position;
        StartCoroutine(Moving());
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Update()
    {
        if (_input.Player.Move.WasPressedThisFrame())
        {
            var dir = _input.Player.Move.ReadValue<Vector2>();
            var angle = Vector2.Angle(transform.up, dir);

            if (_dirs.Count > 2)
                return;
            if(_dirs.Count > 0)
            {
                var lastDir = _dirs.Peek();

                var angle2 = Vector2.Angle(lastDir, dir);
                if (angle2 < 95f)
                    _dirs.Enqueue(dir);
            }
            else
            {
                if (angle < 95f)
                    _dirs.Enqueue(dir);
            }
            //if (angle < 95f)
            //    _dir = dir;
        }
        //_end.position = _trail.GetPosition(0);
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            if(_dirs.Count == 0 && _lastDir.sqrMagnitude == 0)
            {
                yield return null;
                continue;
            }    

            var dir = _dir;
            if (_dirs.Count > 0)
                dir = _dirs.Dequeue();
            else
            {
                dir = _lastDir;
            }
            var nextCellPos = _grid.GetCellInDirection(transform.position, dir);

            if (dir.sqrMagnitude > 0)
                _lastDir = dir;


            transform.rotation = ToQuaternion(dir);

            if (_lastDir.sqrMagnitude > 0f)
            {
                yield return MoveTo(nextCellPos);
                if (PathQueue.Count > 5)
                {
                    var old = PathQueue.Dequeue();
                    var gridPos = _grid.GetGridPosition(PathQueue.Peek().transform.position);
                    _tailPoint.MovingTo(gridPos);
                    old.Destroy();
                }
                var pathGO = new GameObject();
                var point = pathGO.AddComponent<CatPoint>();
                pathGO.transform.position = nextCellPos;
                PathQueue.Enqueue(point);
                var posList = PathQueue.ToList();
                posList.Add(_headPoint);
                posList.Insert(0, _tailPoint);
                _lineRenderer.points = posList;
                _spline.controlPoints = posList.Select(p => p.transform).ToArray();
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator MoveTo(Vector3 gridPos)
    {
        var dist = Vector2.Distance(gridPos, transform.position);
        var lerp = 0f;
        var elapsedTime = 0f;
        var startPos = transform.position;
        while(lerp < 1f)
        {
            elapsedTime += Time.deltaTime;
            lerp = elapsedTime / _timeToCell;
            transform.position = Vector3.Lerp(startPos, gridPos, lerp);
            yield return null;
        }
    }

    private Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(0, 0, angleDir - 90f);
    }
}
