using System.Collections.Generic;
using UnityEngine;

namespace UIPanels
{
    public class ProductionPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform productionGridParent;
        [SerializeField] private ProductInfoButton productionPrefab;
        private readonly List<ProductInfoButton> _productButtons = new();
        public void AddButtonList(List<BoardPieceData> boardPieces)
        {
            foreach (var productInfoButton in _productButtons)
                Destroy(productInfoButton.gameObject);
            _productButtons.Clear();
            foreach (var boardPiece in boardPieces)
            {
                var productInfoButton = Instantiate(productionPrefab, productionGridParent);
                productInfoButton.Init(boardPiece);
                _productButtons.Add(productInfoButton);
            }
        }
    }
}