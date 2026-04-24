using Assets.Source.Scripts.CatLogic;
using UnityEngine;

public class CatPoint : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void DisableCollision()
    {
        var col = GetComponent<Collider2D>();
        Destroy(col);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(1);
        }
    }
}
