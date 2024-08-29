using System.Collections.Generic;
using Board;
using Managers;
using UnityEngine;

[RequireComponent(typeof(BoardGridController))]
public class BoardManager : PersistentSingleton<BoardManager>
{
    [SerializeField] private Transform boardGridParent;
    [SerializeField] private Transform boardPieceParent;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private int rows, columns;
    private readonly Dictionary<Vector2Int, BoardPiece> _boardPieces = new();
    private BoardPiece _piece;
    private BoardGridController _boardGridController;
    public float CellSize => _boardGridController.CellSizeWithSpacing;
    protected override void Awake()
    {
        _boardGridController = GetComponent<BoardGridController>();
        _boardGridController.GenerateGrid(rows, columns, boardGridParent, background.bounds.size);
        for (var row = 0; row < rows; row++)
            for (var column = 0; column < columns; column++)
                _boardPieces.Add(new Vector2Int(row, column), null);
    }
    public void CreateProductPiece(BoardPieceData boardPiece)
    {
        _piece = Instantiate(boardPiece.boardPiecePrefab, boardPieceParent);
        _piece.Initialize(boardPiece);
    }
    public void MoveProductPiece(Vector3 worldPosition)
    {
        _piece.transform.position = _boardGridController.SnapObjectToGrid(worldPosition, _piece.BoardPieceData.pieceSize);
    }
    public bool IsProductInBounds(Vector3 worldPosition)
    {
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _piece.BoardPieceData.pieceSize);

        for (var x = 0; x < _piece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _piece.BoardPieceData.pieceSize.y; y++)
            {
                if (bottomLeftGridCell.x + x > rows - 1 || bottomLeftGridCell.x + x < 0)
                    return false;
                if (bottomLeftGridCell.y + y > columns - 1 || bottomLeftGridCell.y + y < 0)
                    return false;
            }
        return true;
    }
    public bool IsProductPlaceAvailable(Vector3 worldPosition)
    {
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _piece.BoardPieceData.pieceSize);

        for (var x = 0; x < _piece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _piece.BoardPieceData.pieceSize.y; y++)
            {
                if (bottomLeftGridCell.x + x > rows - 1 || bottomLeftGridCell.x + x < 0)
                    return false;
                if (bottomLeftGridCell.y + y > columns - 1 || bottomLeftGridCell.y + y < 0)
                    return false;

                if (_boardPieces[new Vector2Int(bottomLeftGridCell.x + x, bottomLeftGridCell.y + y)] is not null)
                    return false;
            }

        return true;
    }
    public void PlaceProduct(Vector3 worldPosition)
    {
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _piece.BoardPieceData.pieceSize);
        _piece.PutPieceDown(bottomLeftGridCell);
        for (var x = 0; x < _piece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _piece.BoardPieceData.pieceSize.y; y++)
                _boardPieces[new Vector2Int(bottomLeftGridCell.x + x, bottomLeftGridCell.y + y)] = _piece;
    }
    public void DestroyProduct()
    {
        Destroy(_piece.gameObject);
        _piece = null;
    }
    public bool CheckIfSelectedPiece(Vector3 worldPosition)
    {
        var selectedGrid = _boardGridController.GetClosestCell(worldPosition);
        if (_boardPieces.TryGetValue(selectedGrid, out var piece))
        {
            piece.SelectPiece();
            UIManager.Instance.informationPanel.PieceSelected(piece);
            
            return true;
        }
        return false;
    }
}