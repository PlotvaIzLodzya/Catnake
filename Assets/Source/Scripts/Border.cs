using Assets.Source.Scripts.CatLogic;
using UnityEngine;

namespace Assets.Source.Scripts.LevelFeatures
{
    public class Border : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(1);
            }
        }
    }
}