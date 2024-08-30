using System;
using Managers;
using UnityEngine;

public class InputManager : PersistentSingleton<InputManager>
{
    private InputState _inputState = InputState.Idle;
    private void Update()
    {
        switch (_inputState)
        {
            case InputState.Idle:
                IdleUpdate();
                break;
            case InputState.CreatingProduct:
                CreatingProductUpdate();
                break;
            case InputState.PieceSelected:
                PieceSelectedUpdate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void ProductSelected(BoardPieceData boardPiece)
    {
        if (_inputState == InputState.CreatingProduct)
            return;

        
        _inputState = InputState.CreatingProduct;
        
        BoardManager.Instance.CreateProductPiece(boardPiece);
    }
    private void PieceSelectedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider is null) return;

            if (BoardManager.Instance.SelectPiece(worldPosition))
            {
                _inputState = InputState.PieceSelected;
            }
            else _inputState = InputState.Idle;
        }
    }
    private void IdleUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider is null) return;

            if (BoardManager.Instance.SelectPiece(worldPosition))
            {
                _inputState = InputState.PieceSelected;
            }
        }
    }
    private void CreatingProductUpdate()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
            BoardManager.Instance.MoveProductPiece(worldPosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (!BoardManager.Instance.IsProductInBounds(worldPosition))
            {
                BoardManager.Instance.DestroyProduct();
                _inputState = InputState.Idle;
                return;
            }

            if (!BoardManager.Instance.IsProductPlaceAvailable(worldPosition))
            {
                UIManager.Instance.ShowWarning("Product Placement Not Available");
                return;
            }

            BoardManager.Instance.PlaceProduct(worldPosition);
            _inputState = InputState.Idle;
        }
    }
}
internal enum InputState
{
    Idle,
    CreatingProduct,
    PieceSelected,
}