using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSetting")]
public class GameSetting : ScriptableObject
{
    [Tooltip("グリッド1つのサイズ。デフォルトは3で、ボード全体の大きさは3x3=9")]
    [field: SerializeField]
    public int GridSize { get; private set; } = 3;

    [field: SerializeField] public BlockControlMode BlockControlMode { get; private set; } = BlockControlMode.Drag;

    public static Type GetBlockControlInputType(BlockControlMode mode)
    {
        return mode switch
        {
            BlockControlMode.Click => typeof(ClickBlockControlInput),
            BlockControlMode.Drag or _ => typeof(DragBlockControlInput)
        };
    }
}

public enum BlockControlMode
{
    Drag,
    Click
}
