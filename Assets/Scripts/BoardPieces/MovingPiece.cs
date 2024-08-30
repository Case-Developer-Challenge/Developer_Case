public class MovingPiece : BoardPiece
{
    private float _speed;
    private float _damage;
    public override void Initialize(BoardPieceData boardPieceData)
    {
        base.Initialize(boardPieceData);
        _speed = ((MovingBoardPieceData)boardPieceData).speed;
        _damage = ((MovingBoardPieceData)boardPieceData).damage;
    }
    
}