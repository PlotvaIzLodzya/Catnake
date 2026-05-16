using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    [CreateAssetMenu(fileName = nameof(CatVisualPack), menuName = "Cat/Visual/" + nameof(CatVisualPack))]
    public class CatVisualPack : ScriptableObject
    {
        public Sprite Head;
        public Sprite Body;
        public Sprite Tail;
        public Sprite TopPaw;
        public Sprite BotPaw;
    }
}
