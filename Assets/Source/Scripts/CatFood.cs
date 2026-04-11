using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class CatFood : MonoBehaviour
    {
        private float _feedValue = 1f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IFeadable feadable))
            {
                feadable.Feed(_feedValue);
                Destroy();
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
