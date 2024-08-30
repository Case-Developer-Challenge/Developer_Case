using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ProductionManager : PersistentSingleton<ProductionManager>
{
    [SerializeField] private List<BoardPieceData> availableBoardPieces;
    private void Start()
    {
        ResetStartProducts();
    }
    public void ChangeProductPanel(List<BoardPieceData> boardPieceDataList)
    {
        if (boardPieceDataList.Count == 0)
            ResetStartProducts();
        else
            UIManager.Instance.productionPanel.AddButtonList(boardPieceDataList);
    }
    public void ResetStartProducts()
    {
        UIManager.Instance.productionPanel.AddButtonList(availableBoardPieces);
    }
}