using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Source.Scripts.LevelFeatures
{
    public class BorderControl : MonoBehaviour
    {
        [SerializeField] private Border[] _borders;
        [SerializeField] private GameGrid _gameGrid;

        private void Start()
        {
            var positions = _borders.Select(b => (Vector2)b.transform.position).ToArray();
            _gameGrid.Ocupy(positions);
        }
    }
}