using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIPanels
{
    public class ProductionPanel : MonoBehaviour
    {
        private const int TotalVisibleItems = 50;
        private const float ItemHeight = 70;
        public RectTransform content;
        public ScrollRect scrollRect;
        public ProductInfoButton productionPrefab;
        private readonly List<ProductInfoButton> _productButtons = new();
        private List<BoardPieceData> _boardPieces;
        private void SetupScrollView()
        {
            if (_productButtons.Count != 0)
            {
                for (int i = 0; i < TotalVisibleItems; i++)
                    _productButtons[i].Init(_boardPieces[i % _boardPieces.Count]);

                return;
            }

            for (int i = 0; i < TotalVisibleItems; i++)
            {
                var productInfoButton = Instantiate(productionPrefab, content);
                productInfoButton.Init(_boardPieces[i % _boardPieces.Count]);
                _productButtons.Add(productInfoButton);
            }

            scrollRect.content.anchoredPosition = new Vector2(0, scrollRect.content.sizeDelta.y / 2);
        }
        private void Update()
        {
            CheckForScroll();
        }
        private void CheckForScroll()
        {
            if (scrollRect.content.anchoredPosition.y >= ItemHeight) // When scrolling down
            {
                var firstItem = _productButtons[0];
                _productButtons.RemoveAt(0);
                _productButtons.Add(firstItem);
                scrollRect.content.anchoredPosition -= new Vector2(0, ItemHeight);
            }
            else if (scrollRect.content.anchoredPosition.y <= -ItemHeight) // When scrolling up
            {
                var lastItem = _productButtons[^1];
                _productButtons.RemoveAt(_productButtons.Count - 1);
                _productButtons.Insert(0, lastItem);
                scrollRect.content.anchoredPosition += new Vector2(0, ItemHeight);
            }
        }
        public void AddButtonList(List<BoardPieceData> boardPieces)
        {
            _boardPieces = boardPieces;
            SetupScrollView();
        }
    }
}