using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlockData")]
public class BlockData : ScriptableObject
{
    public Vector2Int[] blockCells;
    public int N_Blocks => blockCells.Length;
}
