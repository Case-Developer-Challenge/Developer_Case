using System.Collections.Generic;
using Managers;
using UnityEngine;

public class PieceDataManager : PersistentSingleton<PieceDataManager>
{
    [SerializeField] private List<BoardPieceData> availableBoardPieces;
    private void Start()
    {
        UIManager.Instance.productionPanel.AddButtonList(availableBoardPieces);
    }
}