using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class CatVisuals : MonoBehaviour
    {
        [SerializeField] private CatVisualPack _catVisualPack;
        [SerializeField] private SpriteRenderer _head;
        [SerializeField] private SpriteRenderer _tail;
        [SerializeField] private SpriteRenderer _leftTopPaw;
        [SerializeField] private SpriteRenderer _rightTopPaw;
        [SerializeField] private SpriteRenderer _leftBotPaw;
        [SerializeField] private SpriteRenderer _rightBotPaw;
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _endPoint;
        [SerializeField] private GameObject _ass;

        private void Awake()
        {
            Detach();
            SetVisual(_catVisualPack);
        }

        public void SetVisual(CatVisualPack pack)
        {
            _head.sprite = pack.Head;
            _tail.sprite = pack.Tail;
            _leftTopPaw.sprite = pack.TopPaw;
            _leftBotPaw.sprite = pack.BotPaw;
            _rightTopPaw.sprite = pack.TopPaw;
            _rightBotPaw.sprite = pack.BotPaw;
        }

        public void Detach()
        {
            _head.transform.SetParent(null);
            _body.transform.SetParent(null);
            _endPoint.transform.SetParent(null); 
            _ass.transform.SetParent(null);
        }
    }
}
