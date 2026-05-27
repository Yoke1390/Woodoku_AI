using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BlockData")]
public class BlockData : ScriptableObject
{
    [SerializeField]
    private Vector2Int[] blockCells;
    public int N_Blocks => blockCells.Length;

    private BlockOffset[] _cachedBlockCells;
    public BlockOffset[] BlockCells => _cachedBlockCells;

    private Vector2 _cachedCenter;
    private int _cachedMaxX;
    private int _cachedMaxY;

    public Vector2 Center => _cachedCenter;
    public int MaxX => _cachedMaxX;
    public int MaxY => _cachedMaxY;

    private void OnEnable()
    {
        ResetCache();
    }

    // インスペクターの値が変更されたら呼ばれる
    private void OnValidate()
    {
        // x=0,y=0が起点になるようにオフセット
        OffsetToZero();

        ResetCache();
    }

    private void OffsetToZero()
    {
        if (blockCells == null || blockCells.Length == 0)
            return;

        int minX = blockCells.Min(cell => cell.x);
        if (minX != 0)
            Offset(-minX, 0);

        int minY = blockCells.Min(cell => cell.y);
        if (minY != 0)
            Offset(0, -minY);
    }

    private void Offset(int offsetX, int offsetY)
    {
        for (int i = 0; i < N_Blocks; i++)
        {
            Vector2Int vec = blockCells[i];
            int newX = vec.x + offsetX;
            int newY = vec.y + offsetY;
            blockCells[i] = new Vector2Int(newX, newY);
        }
    }

    private void ResetCache()
    {
        if (blockCells == null || blockCells.Length == 0)
        {
            _cachedBlockCells = Array.Empty<BlockOffset>();
            _cachedCenter = Vector2.zero;
            _cachedMaxX = 0;
            _cachedMaxY = 0;
            return;
        }

        _cachedBlockCells = new BlockOffset[N_Blocks];
        for (int i = 0; i < N_Blocks; i++)
        {
            _cachedBlockCells[i] = (BlockOffset)blockCells[i];
        }

        int maxX = blockCells.Max(cell => cell.x);
        _cachedMaxX = maxX;
        int maxY = blockCells.Max(cell => cell.y);
        _cachedMaxY = maxY;

        float centerX = maxX / 2f;
        float centerY = maxY / 2f;
        var center = new Vector2(centerX, centerY);
        _cachedCenter = center;
    }
}
