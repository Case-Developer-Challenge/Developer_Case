using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPanels
{
    public class ProductInfoButton : MonoBehaviour
    {
        public Button productButton;
        public TMP_Text productNameField;
        private BoardPieceData _boardPieceData;
        public void Init(BoardPieceData boardPieceData)
        {
            _boardPieceData = boardPieceData;
            productNameField.text = boardPieceData.pieceName;
            productButton.onClick.RemoveAllListeners();
            productButton.onClick.AddListener(ProductClicked);
        }
        private void ProductClicked()
        {
            InputManager.Instance.ProductSelected(_boardPieceData);
        }
    }
}