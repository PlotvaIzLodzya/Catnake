using Assets.Source.Scripts.LevelFeatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.CatLogic
{
    public class CatFoodSpawn : MonoBehaviour
    {
        [SerializeField] private GameGrid _grid;
        [SerializeField] private CatFood _catFoodPrefab;

        private List<CatFood> _spawnedCatFood;
        private int _maxCatFoodOnLevel;

        private void Awake()
        {
            _spawnedCatFood = new();
            _maxCatFoodOnLevel = 3;

            StartCoroutine(Spawning());
        }
        private void Update()
        {
            _spawnedCatFood.RemoveAll(c => c == null);
            foreach (var catFood in _spawnedCatFood)
            {
                if (catFood.IsEaten)
                {
                    _grid.FreeCell(catFood.transform.position);
                }
            }

            foreach (var catFood in _spawnedCatFood)
            {
                if (catFood.IsEaten)
                {
                    catFood.Destroy();
                }
            }

        }

        private IEnumerator Spawning()
        {
            while (true)
            {
                yield return new WaitUntil(() => _spawnedCatFood.Count < _maxCatFoodOnLevel);

                var delay = UnityEngine.Random.Range(1.5f, 3f);
                yield return new WaitForSeconds(delay);

                var pos = _grid.GetRandomFreeGridPos();
                _grid.Ocupy(pos);
                var catFood = Instantiate(_catFoodPrefab, pos, Quaternion.identity);
                _spawnedCatFood.Add(catFood);
            }
        }

    }
}
