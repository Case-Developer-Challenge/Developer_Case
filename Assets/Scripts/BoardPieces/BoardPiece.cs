using TMPro;
using UnityEngine;

public abstract class BoardPiece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private TMP_Text nameTmp;
    private float _health;
    public BoardPieceData BoardPieceData { get; private set; }
    public Vector2Int BottomLeftCell { get; private set; }
    public virtual void Initialize(BoardPieceData boardPieceData)
    {
        BoardPieceData = boardPieceData;
        nameTmp.text = boardPieceData.name;
        // image.sprite = boardPieceData.pieceSprite;
        transform.localScale = new Vector2(boardPieceData.pieceSize.x * BoardManager.Instance.CellSize,
            boardPieceData.pieceSize.y * BoardManager.Instance.CellSize);
    }
    public virtual void PutPieceDown(Vector2Int bottomLeftGridCell)
    {
        BottomLeftCell = bottomLeftGridCell;
    }
    public virtual void SelectPiece()
    {
        image.color = Color.red;

        ProductionManager.Instance.ChangeProductPanel(BoardPieceData.products);
    }
    public virtual void DeselectPiece()
    {
        image.color = Color.white;
        ProductionManager.Instance.ResetStartProducts();
    }
}