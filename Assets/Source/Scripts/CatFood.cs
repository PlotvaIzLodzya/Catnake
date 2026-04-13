using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class CatFood : MonoBehaviour
    {
        private float _feedValue = 1f;

        public bool IsEaten { get; private set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IFeadable feadable))
            {
                feadable.Feed(_feedValue);
                IsEaten = true;
                gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
