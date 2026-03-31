using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotationSpring2D : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Целевой угол, к которому стремится объект (в градусах)")]
    public float targetAngle = 0f;

    [Tooltip("Жесткость пружины. Чем выше, тем сильнее тянет к цели.")]
    public float stiffness = 50f;

    [Tooltip("Затухание. Помогает убрать колебания.")]
    public float damping = 5f;

    [Tooltip("Если true, удерживает угол относительно мира. Если false - относительно начального поворота.")]
    public bool useWorldAngle = true;

    [Tooltip("Подключенное тело (если нужно удерживать угол относительно другого объекта).")]
    public Rigidbody2D connectedBody;

    private Rigidbody2D rb;
    private float startAngle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startAngle = transform.rotation.eulerAngles.z;
    }

    void FixedUpdate()
    {
        float currentAngle = transform.rotation.eulerAngles.z;
        float target = targetAngle;

        // Если нужно удерживать угол относительно другого объекта
        if (connectedBody != null)
        {
            float otherAngle = connectedBody.transform.rotation.eulerAngles.z;
            // Вычисляем разницу, чтобы удерживать относительный поворот
            target = otherAngle + targetAngle;
        }
        else if (!useWorldAngle)
        {
            // Удерживаем начальный локальный поворот
            target = startAngle + targetAngle;
        }

        // Mathf.DeltaAngle правильно обрабатывает переход через 360/0 градусов
        float angleDiff = Mathf.DeltaAngle(currentAngle, target);

        // Формула ПД-регулятора (Пружина + Демпфер)
        // Сила = (Разница углов * Жесткость) - (Текущая скорость * Затухание)
        float torque = (angleDiff * stiffness) - (rb.angularVelocity * damping);

        rb.AddTorque(torque * Time.fixedDeltaTime);
        rb.MovePosition(connectedBody.position);
    }

    // Для отладки в редакторе
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Debug.DrawLine(transform.position, transform.position + transform.up * 2, Color.yellow);
    }
}
