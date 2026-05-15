using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BlockData")]
public class BlockData : ScriptableObject
{
    [SerializeField]
    private Vector2Int[] blockCells;
    public int N_Blocks => blockCells.Length;

    private BoardPosition[] _cachedBlockCells = null;
    public IReadOnlyList<BoardPosition> BlockCells
    {
        get
        {
            if (_cachedBlockCells == null)
            {
                _cachedBlockCells = new BoardPosition[N_Blocks];
                for (int i = 0; i < N_Blocks; i++)
                {
                    _cachedBlockCells[i] = blockCells[i];
                }
            }
            return _cachedBlockCells;
        }
    }

    private Vector2? _cachedCenter = null;

    public Vector2 Center
    {
        get
        {
            if (_cachedCenter.HasValue)
            {
                return _cachedCenter.Value;
            }

            if (blockCells == null || blockCells.Length == 0)
            {
                return Vector2.zero;
            }

            int minX = blockCells.Min(cell => cell.x);
            int minY = blockCells.Min(cell => cell.y);
            int maxX = blockCells.Max(cell => cell.x);
            int maxY = blockCells.Max(cell => cell.y);

            float centerX = (maxX + minX) / 2f;
            float centerY = (maxY + minY) / 2f;

            _cachedCenter = new Vector2(centerX, centerY);
            return _cachedCenter.Value;
        }
    }

    // インスペクターの値が変更されたら呼ばれる
    private void OnValidate()
    {
        // 値が変わったので、次回呼ばれた時に作り直すように null に戻しておく
        _cachedBlockCells = null;
        _cachedCenter = null;
    }
}
