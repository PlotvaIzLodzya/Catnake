using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer<T> where T : struct
{
    private T _value;
    private InputAction _inputAction;

    public float BufferTime { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsBuffered { get; private set; }

    public bool IsPressed => _inputAction.IsPressed();

    public InputBuffer(InputAction inputAction, float bufferTime)
    {
        _inputAction = inputAction;
        BufferTime = bufferTime;
    }

    public T Buffer()
    {
        ElapsedTime = 0f;
        IsBuffered = true;
        _value = _inputAction.ReadValue<T>();
        
        return _value;
    }

    public T Consume()
    {
        IsBuffered = false;
        ElapsedTime = 0f;

        return _value;
    }

    public void Tick(float deltaTime)
    {
        if (IsPressed)
            Buffer();

        if (IsBuffered)
        {
            ElapsedTime += deltaTime;
            IsBuffered = ElapsedTime <= BufferTime;
        }
    }

    public void Clear()
    {
        IsBuffered = false;
        ElapsedTime = 0f;
        _value = default;
    }
}
