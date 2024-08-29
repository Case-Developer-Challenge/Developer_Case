using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPanels
{
    public class ProductInfo : MonoBehaviour
    {
        public TMP_Text productNameField;
        public Image image;
        private BoardPieceData _boardPieceData;
        public void Init(BoardPieceData boardPieceData)
        {
            _boardPieceData = boardPieceData;
            productNameField.text = boardPieceData.pieceName;
            image.sprite = _boardPieceData.pieceSprite;
            
        }
    }
}