using System.Collections;
using UnityEngine;

public class Follower : GridMover
{
    [SerializeField] private GameGrid _grid;
    public GridMover FollowTo;
    public override Vector3 LastPos { get; set; }
    public override float TimeToCell => FollowTo.TimeToCell;

    private void Start()
    {
        StartCoroutine(Following());
    }

    private IEnumerator Following()
    {
        while (true)
        {
            yield return MoveTo(FollowTo.LastPos);
            LastPos = transform.position;
        }
    }

    private IEnumerator MoveTo(Vector3 gridPos)
    {
        var dist = Vector2.Distance(gridPos, transform.position);
        var lerp = 0f;
        var elapsedTime = 0f;
        var startPos = transform.position;
        while (lerp < 1f)
        {
            elapsedTime += Time.deltaTime;
            lerp = elapsedTime / FollowTo.TimeToCell;
            var pos = Vector3.Lerp(startPos, gridPos, lerp);
            pos = _grid.GetGridPosition(pos);
            transform.position = pos;
            yield return null;
        }
    }
}
