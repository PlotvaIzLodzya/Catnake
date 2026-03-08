using System;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private GameGrid _grid;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed;

    private PlayerInput _input;
    private InputBuffer<Vector2> _moveBuffer;
    private Vector3 _dir;

    void Awake()
    {
        _input = new PlayerInput();
        _moveBuffer = new InputBuffer<Vector2>(_input.Player.Move, 0.2f);
        
        transform.position = _grid.GetGridPosition(transform.position);
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
        _moveBuffer.Tick(Time.deltaTime);
        if (_grid.DistToCellCenterNormalized(transform.position) >= 0.9f && _grid.IsOnCellEnter(transform.position, _dir))
        {
            if (_moveBuffer.IsBuffered)
            {
                var inputDirection = _moveBuffer.Consume();
                
                var angle = Vector2.Angle(transform.up, inputDirection);
                
                if(inputDirection != (Vector2)transform.up && angle < 160)
                {
                    _dir = inputDirection;
                    transform.position = _grid.GetGridPosition(transform.position);
                    transform.rotation = ToQuaternion(_dir);
                }
            }
        }

        var closestCell = _grid.GetCellInDirection(transform.position, _dir);
        var dir = (closestCell - transform.position).normalized;
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private Quaternion ToQuaternion(Vector2 dir)
    {
        float angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        return Quaternion.Euler(0, 0, angleDir - 90f);
    }
}
