using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class BoardGridController : MonoBehaviour
    {
        [SerializeField] private GameObject gridCellPrefab;
        [SerializeField] private float cellSpacing = 5f;
        private readonly Dictionary<Vector2Int, Vector3> _boardWorldPosition = new();
        private Vector2 _firstPieceOffset;
        private int _rows;
        private int _columns;
        public float CellSizeWithSpacing { get; private set; }
        public void GenerateGrid(int rows, int columns, Transform gridParent, Vector3 boundsSize)
        {
            _rows = rows;
            _columns = columns;
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            var cellWidth = (boundsSize.x - (_columns - 1) * cellSpacing) / _columns;
            var cellHeight = (boundsSize.y - (_rows - 1) * cellSpacing) / _rows;

            var cellSize = Mathf.Min(cellHeight, cellWidth);
            CellSizeWithSpacing = cellSize + cellSpacing;

            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _columns; col++)
                {
                    var gridCell = Instantiate(gridCellPrefab, gridParent);

                    gridCell.transform.localScale = Vector3.one * cellSize;
                    var posX = col * (cellSize + cellSpacing) - boundsSize.x / 2 + cellSize / 2;
                    var posY = row * (cellSize + cellSpacing) - boundsSize.y / 2 + cellSize / 2;

                    gridCell.transform.localPosition = new Vector3(posX, posY, -.1f);
                    if (row == 0 && col == 0)
                        _firstPieceOffset = gridCell.transform.position;
                    _boardWorldPosition.Add(new Vector2Int(col, row), gridCell.transform.position);
                }
            }
        }
        public Vector2 SnapObjectToGrid(Vector2 objectPosition, Vector2Int objectSize)
        {
            var objectPosInGridWorld = objectPosition - _firstPieceOffset;

            var cellOffset = (objectSize - Vector2.one) * CellSizeWithSpacing / 2;
            var bottomLeft = objectPosInGridWorld - cellOffset;
            var topRight = objectPosInGridWorld + cellOffset;

            var bottomLeftCell = GetClosestCell(bottomLeft);
            var topRightCell = GetClosestCell(topRight);

            if (IsValidGridCell(bottomLeftCell) && IsValidGridCell(topRightCell))
            {
                var snappedPosition = BoardToWorldPosition(topRightCell) - new Vector3((objectSize.x - 1) * CellSizeWithSpacing * .5f,
                    (objectSize.y - 1) * CellSizeWithSpacing * .5f, 0);
                return snappedPosition;
            }

            return objectPosition;
        }
        public Vector2Int GetBottomLeftGridCell(Vector2 objectPosition, Vector2Int objectSize)
        {
            var objectPosInGridWorld = objectPosition - _firstPieceOffset;

            var cellOffset = (objectSize - Vector2.one) * CellSizeWithSpacing / 2;
            var bottomLeft = objectPosInGridWorld - cellOffset;

            return GetClosestCell(bottomLeft);  
        }
        public Vector2Int GetClosestCell(Vector2 position, bool withOffset = false)
        {
            // Find the closest grid cell based on object position
            var closestCol = Mathf.RoundToInt(((withOffset ? -_firstPieceOffset.x : 0) + position.x) / CellSizeWithSpacing);
            var closestRow = Mathf.RoundToInt(((withOffset ? -_firstPieceOffset.y : 0) + position.y) / CellSizeWithSpacing);

            return new Vector2Int(closestCol, closestRow);
        }
        public bool IsValidGridCell(Vector2Int cell)
        {
            return _boardWorldPosition.ContainsKey(cell);
        }
        private Vector3 BoardToWorldPosition(Vector2Int boardPos)
        {
            return _boardWorldPosition[boardPos];
        }
    }
}