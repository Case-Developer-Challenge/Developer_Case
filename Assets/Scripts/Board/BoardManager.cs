using System.Collections.Generic;
using Board;
using Managers;
using UnityEngine;

[RequireComponent(typeof(BoardGridController))]
[RequireComponent(typeof(BoardVisualController))]
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
    private BoardVisualController _boardVisualController;
    public float CellSize => _boardGridController.CellSizeWithSpacing;
    protected override void Awake()
    {
        _boardGridController = GetComponent<BoardGridController>();
        _boardVisualController = GetComponent<BoardVisualController>();
        _boardGridController.GenerateGrid(rows, columns, boardGridParent, background);
        _boardVisualController.SetCameraPosition(background.size);
        for (var row = 0; row < columns; row++)
            for (var column = 0; column < rows; column++)
                _boardPieces.Add(new Vector2Int(row, column), null);
    }
    public void CreateProductPiece(BoardPieceData boardPiece)
    {
        DeselectCurrentPiece();
        _createdPiece = PoolManager.Instance.GetFromPool(boardPiece.boardPiecePrefab, boardPiece.name, boardPieceParent);
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
                if (bottomLeftGridCell.x + x > columns - 1 || bottomLeftGridCell.x + x < 0)
                    return false;
                if (bottomLeftGridCell.y + y > rows - 1 || bottomLeftGridCell.y + y < 0)
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
                if (bottomLeftGridCell.x + x > columns - 1 || bottomLeftGridCell.x + x < 0)
                    return false;
                if (bottomLeftGridCell.y + y > rows - 1 || bottomLeftGridCell.y + y < 0)
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
        PoolManager.Instance.ReturnToPool(_createdPiece, _createdPiece.name);
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
        {
            if (_selectedPiece == piece)
                return false;
            return _selectedPiece.SetTarget(targetGrid, piece);
        }

        return false;
    }
    public bool IsPieceEmpty(Vector2Int pos)
    {
        return _boardPieces.ContainsKey(pos) && _boardPieces[pos] == null;
    }
    public void MovePieceFrom(Vector2Int startLeftBottomPoint, Vector2Int moveLeftBottomPoint, BoardPiece piece)
    {
        var direction = moveLeftBottomPoint - startLeftBottomPoint;
        var newPositionsList = new List<Vector2Int>();
        for (var x = 0; x < piece.BoardPieceData.pieceSize.x; x++)
        {
            for (var y = 0; y < piece.BoardPieceData.pieceSize.y; y++)
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
    public void PieceDestroyed(BoardPiece boardPiece)
    {
        if (_selectedPiece == boardPiece)
            DeselectCurrentPiece();
        for (var x = 0; x < boardPiece.BoardPieceData.pieceSize.x; x++)
            for (var y = 0; y < boardPiece.BoardPieceData.pieceSize.y; y++)
                _boardPieces[new Vector2Int(boardPiece.BottomLeftCell.x + x, boardPiece.BottomLeftCell.y + y)] = null;
    }
    public bool CheckIfNeighbor(BoardPiece piece, BoardPiece neighbor)
    {
        var piecePosition = piece.BottomLeftCell;
        var pieceSize = piece.BoardPieceData.pieceSize;

        var directions = new List<Vector2Int> { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

        foreach (var direction in directions)
            for (var x = 0; x < pieceSize.x; x++)
                for (var y = 0; y < pieceSize.y; y++)
                {
                    var checkCell = piecePosition + direction + new Vector2Int(x, y);
                    if (_boardGridController.IsValidGridCell(checkCell) && _boardPieces[checkCell] == neighbor)
                        return true;
                }

        return false;
    }
    public List<Vector2Int> GetSurroundingEmptySpots(BoardPiece piece)
    {
        var emptySpots = new List<Vector2Int>();
        var piecePosition = piece.BottomLeftCell;
        var pieceSize = piece.BoardPieceData.pieceSize;

        var directions = new List<Vector2Int> { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

        foreach (var direction in directions)
            for (var x = 0; x < pieceSize.x; x++)
                for (var y = 0; y < pieceSize.y; y++)
                {
                    var checkCell = piecePosition + direction + new Vector2Int(x, y);
                    if (_boardGridController.IsValidGridCell(checkCell) && _boardPieces[checkCell] == null)
                        emptySpots.Add(checkCell);
                }

        return emptySpots;
    }
    private void DeselectCurrentPiece()
    {
        if (_selectedPiece is null) return;

        _selectedPiece.DeselectPiece();
        UIManager.Instance.informationPanel.PieceDeSelected();
        _selectedPiece = null;
    }
}