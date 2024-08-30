using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PoolManager : PersistentSingleton<PoolManager>
    {
        private readonly Dictionary<string, Queue<BoardPiece>> _pools = new();
        private void CreatePool(BoardPiece piecePrefab, int initialSize)
        {
            var poolKey = piecePrefab.name;

            if (_pools.ContainsKey(poolKey)) return;

            _pools[poolKey] = new Queue<BoardPiece>();
            for (var i = 0; i < initialSize; i++)
            {
                var newPiece = Instantiate(piecePrefab);
                newPiece.gameObject.SetActive(false);
                _pools[poolKey].Enqueue(newPiece);
            }
        }
        public BoardPiece GetFromPool(BoardPiece piecePrefab, Transform parent = null)
        {
            var poolKey = piecePrefab.name;

            if (!_pools.ContainsKey(poolKey))
                CreatePool(piecePrefab, 10);

            if (_pools[poolKey].Count == 0)
            {
                var newPiece = Instantiate(piecePrefab, parent);
                newPiece.gameObject.SetActive(false);
                return newPiece;
            }

            var pieceToReturn = _pools[poolKey].Dequeue();
            pieceToReturn.transform.parent = parent;
            pieceToReturn.gameObject.SetActive(true);
            return pieceToReturn;
        }
        public void ReturnToPool(BoardPiece piece)
        {
            var poolKey = piece.name;

            // Deactivate the piece and return it to the pool
            piece.gameObject.SetActive(false);

            if (!_pools.ContainsKey(poolKey))
            {
                _pools[poolKey] = new Queue<BoardPiece>();
            }

            _pools[poolKey].Enqueue(piece);
        }
    }
}