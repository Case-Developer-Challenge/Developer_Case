using System;
using System.Collections;
using UnityEngine;

public class MovingPiece : BoardPiece
{
    private const float AttackInterval = .6f;
    private float _speed;
    private float _damage;
    private Coroutine _moveCoroutine;
    private Vector2Int _targetGrid;
    private BoardPiece _targetPiece;
    public override void Initialize(BoardPieceData boardPieceData)
    {
        base.Initialize(boardPieceData);
        _speed = ((MovingBoardPieceData)boardPieceData).speed;
        _damage = ((MovingBoardPieceData)boardPieceData).damage;
    }
    public override bool SetTarget(Vector2Int selectedGrid, BoardPiece targetPiece)
    {
        if (targetPiece is null)
        {
            if (_moveCoroutine is not null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveCoroutine());
            _targetGrid = selectedGrid;
        }
        else // attack if target piece is not null
        {
            if (_moveCoroutine is not null)
                StopCoroutine(_moveCoroutine);

            _targetPiece = targetPiece;
            _moveCoroutine = StartCoroutine(AttackCoroutine());
        }

        return false;
    }
    private IEnumerator MoveCoroutine()
    {
        var waitDuration = new WaitForSeconds(5 / _speed);
        while (gameObject.activeSelf)
        {
            yield return waitDuration;
            var path = PathFind.FindPath(BottomLeftCell, _targetGrid);
            if (path is null) // not reachable stop movement
                yield break;

            BoardManager.Instance.MovePieceFrom(BottomLeftCell, path[1], this);
            if (path.Count == 2) // reached target
                yield break;
        }
    }
    private IEnumerator AttackCoroutine()
    {
        do
        {
            var surroundingEmptySpots = BoardManager.Instance.GetSurroundingEmptySpots(_targetPiece);
            // sorting empty spots for attacking the closest
            surroundingEmptySpots.Sort((a, b) =>
            {
                var pathLengthA = PathFind.CalculatePathLength(BottomLeftCell, a);
                var pathLengthB = PathFind.CalculatePathLength(BottomLeftCell, b);

                return pathLengthA.CompareTo(pathLengthB);
            });
            if (surroundingEmptySpots.Count == 0) // target not Reachable
                yield break;
            if (BoardManager.Instance.CheckIfNeighbor(_targetPiece, this)) // doesn't need to move if already in position
                break;

            _targetGrid = surroundingEmptySpots[0];
            yield return MoveCoroutine();
        } while (_targetGrid != BottomLeftCell);

        var waitDuration = new WaitForSeconds(AttackInterval);
        while (_targetPiece.IsActive)
        {
            yield return waitDuration;
            _targetPiece.DamagePiece(_damage);
        }
    }
}