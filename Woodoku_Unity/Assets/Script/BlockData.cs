using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlockData")]
public class BlockData : ScriptableObject
{
    [SerializeField]
    private Vector2Int[] blockCells;

    public IReadOnlyList<Vector2Int> BlockCells => blockCells;
    public int N_Blocks => blockCells.Length;
}
