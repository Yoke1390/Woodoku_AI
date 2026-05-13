using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BlockData")]
public class BlockData : ScriptableObject
{
    [SerializeField]
    private Vector2Int[] blockCells;

    public IReadOnlyList<Vector2Int> BlockCells => blockCells;
    public int N_Blocks => blockCells.Length;

    private Vector2? cachedCenter = null;

    public Vector2 Center
    {
        get
        {
            if (cachedCenter.HasValue)
            {
                return cachedCenter.Value;
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

            cachedCenter = new Vector2(centerX, centerY);
            return cachedCenter.Value;
        }
    }
}
