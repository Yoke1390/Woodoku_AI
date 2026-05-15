using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSetting")]
public class GameSetting : ScriptableObject
{
    [Tooltip("グリッド1つのサイズ。デフォルトは3で、ボード全体の大きさは3x3=9")]
    [field: SerializeField]
    public int GridSize { get; private set; } = 3;
}
