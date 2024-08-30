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
    private BoardPiece _createdPiece;
    private BoardPiece _selectedPiece;
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
        DeselectCurrentPiece();
        _createdPiece = Instantiate(boardPiece.boardPiecePrefab, boardPieceParent);
        _createdPiece.Initialize(boardPiece);
    }
    public void MoveProductPiece(Vector3 worldPosition)
    {
        _createdPiece.transform.position = _boardGridController.SnapObjectToGrid(worldPosition, _createdPiece.BoardPieceData.pieceSize);
    }
    public bool IsProductInBounds(Vector3 worldPosition)
    {
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _createdPiece.BoardPieceData.pieceSize);

        for (var x = 0; x < _createdPiece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _createdPiece.BoardPieceData.pieceSize.y; y++)
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
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _createdPiece.BoardPieceData.pieceSize);

        for (var x = 0; x < _createdPiece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _createdPiece.BoardPieceData.pieceSize.y; y++)
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
        var bottomLeftGridCell = _boardGridController.GetBottomLeftGridCell(worldPosition, _createdPiece.BoardPieceData.pieceSize);
        _createdPiece.PutPieceDown(bottomLeftGridCell);
        for (var x = 0; x < _createdPiece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < _createdPiece.BoardPieceData.pieceSize.y; y++)
                _boardPieces[new Vector2Int(bottomLeftGridCell.x + x, bottomLeftGridCell.y + y)] = _createdPiece;
    }
    public void DestroyProduct()
    {
        Destroy(_createdPiece.gameObject);
        _createdPiece = null;
    }
    public bool SelectPiece(Vector3 worldPosition)
    {
        var selectedGrid = _boardGridController.GetClosestCell(worldPosition, true);

        if (_boardPieces.TryGetValue(selectedGrid, out var piece))
        {
            if (piece is null || _selectedPiece == piece)
            {
                DeselectCurrentPiece();
                return false;
            }

            DeselectCurrentPiece();
            _selectedPiece = piece;
            _selectedPiece.SelectPiece();
            UIManager.Instance.informationPanel.PieceSelected(_selectedPiece);
            return true;
        }

        //if not in bounds return the current state 
        return _selectedPiece is not null;
    }
    public bool SetTargetToSelectedPiece(Vector3 worldPosition)
    {
        var targetGrid = _boardGridController.GetClosestCell(worldPosition, true);

        if (_boardPieces.TryGetValue(targetGrid, out var piece))
            return _selectedPiece.SetTarget(targetGrid, piece);

        return false;
    }
    public bool IsPieceEmpty(Vector2Int pos)
    {
        return _boardPieces.ContainsKey(pos) && _boardPieces[pos] == null;
    }
    private void DeselectCurrentPiece()
    {
        if (_selectedPiece is null) return;

        _selectedPiece.DeselectPiece();
        UIManager.Instance.informationPanel.PieceDeSelected();
        _selectedPiece = null;
    }
    public void MovePieceFrom(Vector2Int startLeftBottomPoint, Vector2Int moveLeftBottomPoint, BoardPiece piece)
    {
        Vector2Int direction = moveLeftBottomPoint - startLeftBottomPoint;
        var newPositionsList = new List<Vector2Int>();
        for (int x = 0; x < piece.BoardPieceData.pieceSize.x; x++)
        {
            for (int y = 0; y < piece.BoardPieceData.pieceSize.y; y++)
            {
                var oldPosition = new Vector2Int(piece.BottomLeftCell.x + x, piece.BottomLeftCell.y + y);
                newPositionsList.Add(oldPosition + direction);
                _boardPieces[oldPosition] = null;
            }
        }

        foreach (var newPosition in newPositionsList)
            _boardPieces[newPosition] = piece;
        piece.PutPieceDown(moveLeftBottomPoint);
        piece.transform.position += (Vector3)(_boardGridController.CellSizeWithSpacing * (Vector2)direction);
    }
}