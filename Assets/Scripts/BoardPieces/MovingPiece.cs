using System.Collections;
using UnityEngine;

public class MovingPiece : BoardPiece
{
    private float _speed;
    private float _damage;
    private Coroutine _moveCoroutine;
    private Vector2Int _targetGrid;
    public override void Initialize(BoardPieceData boardPieceData)
    {
        base.Initialize(boardPieceData);
        _speed = ((MovingBoardPieceData)boardPieceData).speed;
        _damage = ((MovingBoardPieceData)boardPieceData).damage;
    }
    public override bool SetTarget(Vector2Int selectedGrid, BoardPiece piece)
    {
        if (piece is null)
        {
            if (_moveCoroutine is not null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveCoroutine());
            _targetGrid = selectedGrid;
        }
        else // attack if piece is not null
        {
        }

        return false;
    }
    private IEnumerator MoveCoroutine()
    {
        var waitDuration = new WaitForSeconds(10 / _speed);
        while (gameObject.activeSelf)
        {
            yield return waitDuration;
            var path = PathFind.FindPath(BottomLeftCell, _targetGrid);
            if (path is null) // not reachable stop movement
                yield break;

            BoardManager.Instance.MovePieceFrom(BottomLeftCell, path[1], this);
        }
    }
}