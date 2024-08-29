using System.Collections.Generic;
using UnityEngine;

namespace UIPanels
{
    public class InformationPanel : MonoBehaviour
    {
        [SerializeField] private ProductInfo selectedProductInfo;
        [SerializeField] private RectTransform productInfoParent;
        [SerializeField] private ProductInfo productInfoPrefab;
        private readonly List<ProductInfo> _createdProductInfos = new();
        private void Awake()
        {
            selectedProductInfo.gameObject.SetActive(false);
        }
        public void PieceSelected(BoardPiece selectedPiece)
        {
            selectedProductInfo.gameObject.SetActive(true);
            selectedProductInfo.Init(selectedPiece.BoardPieceData);
            foreach (var createdProductInfo in _createdProductInfos)
                Destroy(createdProductInfo.gameObject);
            _createdProductInfos.Clear();
            foreach (var boardPieceData in selectedPiece.BoardPieceData.products)
            {
                var productInfo = Instantiate(productInfoPrefab, productInfoParent);
                productInfo.Init(boardPieceData);
                _createdProductInfos.Add(productInfo);
            }
        }
        public void PieceDeSelected()
        {
            selectedProductInfo.gameObject.SetActive(false);
            foreach (var createdProductInfo in _createdProductInfos)
                Destroy(createdProductInfo.gameObject);
            _createdProductInfos.Clear();

        }
    }
}