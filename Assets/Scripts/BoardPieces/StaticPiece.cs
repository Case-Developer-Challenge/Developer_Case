
using UnityEngine;

public class StaticPiece : BoardPiece
{
    public override bool SetTarget(Vector2Int selectedGrid, BoardPiece targetPiece)
    {
        return false;
    }
}