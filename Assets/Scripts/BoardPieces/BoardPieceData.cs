using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardPiece", menuName = "GameData", order = 0)]
public class BoardPieceData : ScriptableObject
{
    public string pieceName;
    public Sprite pieceSprite;
    public Vector2Int pieceSize;
    public BoardPiece boardPiecePrefab;
    public int health;
    public List<BoardPieceData> products;
}