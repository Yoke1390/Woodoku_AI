現在の状態を確認します。

修正内容を確認しました。`TryScreenPointToBoardPosition` を `BoardUI` に移したのは綺麗な解決です。前回指摘した全ての項目に対応されています。

ロードマップ全体を見据えてレビューします。

---

## 前回からの修正点

| 項目 | 状態 |
|---|---|
| `Reset()` の名前衝突 | ✅ `ResetBlock()` に改名、private化 |
| `ellSize` タイポ | ✅ 修正＋座標変換を `BoardUI` に移譲 |
| Singleton 依存の戻り | ✅ `boardUI` フィールド経由に |
| `blockBaseBoardPostion` タイポ | ✅ 修正 |
| 不要な using | ✅ 削除 |

**設計的に最良の変化**は座標変換を `BoardUI.TryScreenPointToBoardPosition` として「PointerEventData を知らない、純粋な座標変換 API」にしたこと。これで `BoardUI` の責務が「描画 + 座標系の知識」に綺麗に閉じました。

---

## 全体設計の現状評価

```
DraggableBlock (UI入力)
    ↓ DropHandler（注入されたコールバック）
WoodokuGameManager (調整役)
    ├─→ boardUI.TryScreenPointToBoardPosition (UI座標 → 盤面座標)
    └─→ boardData.CanPlaceBlock / SetCell (純粋データ操作)
            ↓ CellUpdate イベント
        boardUI.UpdateCellState (描画反映)
```

データ層 → イベント → UI層 という流れと、UI 入力 → コールバック → 調整役 → データ層 という流れが、**矢印の向きで明確に区別**されています。ROADMAP 冒頭の「内部のデータ管理と画面の描画を明確に分ける」方針に沿っています。

---

## 🟡 残っている小さな課題

### ① `HandBlock` だけ Singleton 依存が残っている

[HandBlock.cs:24](Woodoku_Unity/Assets/Script/HandBlock.cs#L24)

```csharp
float cellSize = BoardUI.Instance.CellSize;
```

`DraggableBlock` の Singleton 排除で築いた構造を、ここだけ崩しています。

**ヒント:** `HandBlock.Initialize(BlockData blockData, float cellSize)` のように外から渡す形にし、`HandManager.GenerateHandBlock` が `boardUI` から取って渡せます。`HandManager` 側も `Initialize` で `boardUI` の参照（または `cellSize`）を受け取るようにすると一貫します。

### ② `BoardData.CanPlaceBlock` 内の `Debug.Log`

[BoardData.cs:69](Woodoku_Unity/Assets/Script/BoardData.cs#L69)

```csharp
Debug.Log($"Target Pos: {targetPos.x}, {targetPos.y} value: {cellValue}");
```

§7 の手詰まり判定では、このメソッドを毎フレーム数百回呼ぶ可能性があります。デバッグ用なら `#if UNITY_EDITOR` で囲むか、削除を検討してください。

### ③ vestigial（使われなくなった）Singleton

`WoodokuGameManager.Instance` は現在のコードから外部参照がなくなっています。`BoardUI.Instance` も `HandBlock` を直せば消せます。

**ヒント:** Singleton は「使われ続けると除去が困難になる」依存です。今のうちに削除しておくか、明示的に「残す理由」を `[SerializeField]` のコメントで残すと、後から読む人（または AI 連携時の自分）が判断しやすくなります。

### ④ `WoodokuGameManager.GetBlockBaseBoardPosition` は薄いラッパー

```csharp
private bool GetBlockBaseBoardPosition(
    PointerEventData eventData,
    Vector2 centerCellOffset,
    out BoardPosition blockBaseBoardPosition
) {
    return boardUI.TryScreenPointToBoardPosition(...);  // 単純な転送
}
```

`HandleDropRequest` 内に直接 `boardUI.TryScreenPointToBoardPosition(...)` を書いたほうがシンプルかもしれません。間接化が意味のある抽象になっていれば残す価値がありますが、現状は転送だけです。

---

## 🔭 ロードマップを踏まえた設計提言

ここからが本題です。今後の §5 後半〜§7 に向けて、構造的に考えておくべき論点を挙げます。

### A. §5/§6: ブロック配置と消去判定の責務配置

ROADMAP §6 には「盤面全体をスキャンする処理を `BoardData` クラスに実装する」とあります。これは正しい判断で、**「データの整合性」は `BoardData` が責任を持つ**べきです。

提案する API 構造（骨格）:

```csharp
public class BoardData
{
    // 配置 + 消去判定 + クリアまでを一気通貫で
    public PlacementResult PlaceBlock(BlockData blockData, BoardPosition basePos);

    // 結果を呼び出し側に返す
    public readonly struct PlacementResult
    {
        public bool Success { get; }
        public IReadOnlyList<LineCleared> ClearedLines { get; }  // スコア計算用
    }
}
```

ポイント:
- `WoodokuGameManager.PlaceBlock`（現在 `private`）の処理を `BoardData` に移譲する
- 消去対象セルへの `SetCell(pos, 0)` で既存の `CellUpdate` イベントが発火 → UI 更新が自動的に走る
- `PlacementResult` を返すことで、将来「何ライン消えたか」をスコア・エフェクトに渡せる

### B. §7: 手札の状態管理

現在 `DraggableBlock` が成功時に `Destroy(gameObject)` していますが、`HandManager` は誰も消費されたことを知りません。§7 で「3つ消費されたら補充」を実装するには、この情報を伝達する必要があります。

提案する流れ:

```
DraggableBlock.OnEndDrag (成功)
   ↓ Destroy する前にコールバックで通知
HandManager.OnBlockConsumed(slotIndex)
   ↓ 内部カウンタ更新、空になったらイベント発火
HandManager.HandEmpty イベント
   ↓
WoodokuGameManager がリスナーとして補充をトリガー
```

ここで `DropHandler` を拡張するか、別の `OnBlockConsumed` コールバックを追加するか選択肢があります。**`DropHandler` を `bool` 返却に留め、消費通知は別経路にする**のが SRP 的に綺麗です。

### C. §7: 手詰まり判定の置き場所

「全ブロック × 全座標で配置可能かをシミュレーション」というロジックは:

- **データ層**: 配置可能性は `BoardData.CanPlaceBlock` で判定できる
- **集約**: 「現在の手札 × 盤面で、どこかに置けるか?」という問いは `WoodokuGameManager` が答えるのが自然（手札と盤面の両方を知っている唯一の存在）

```csharp
// WoodokuGameManager
public bool HasAnyValidPlacement(IEnumerable<BlockData> currentHand);
```

このメソッドが `false` を返したらゲームオーバー、というシンプルな構造になります。

### D. AI 連携の道筋（先取り）

ROADMAP の最終目標である AI 環境化に向けて、現状を評価すると:

- ✅ `WoodokuGameManager.TryPlaceBlock(BlockData, BoardPosition)` という公開 API がある
- ✅ `BoardData` が MonoBehaviour ではない純粋クラス（テスト・シミュレーション可能）
- ❌ 現在の手札を取得する公開 API がない（AI は手札を見て手を決める必要がある）
- ❌ ゲーム状態（進行中/ゲームオーバー）を取得する公開 API がない

§7 完了時には、AI 用に最低限以下が必要になります:

```csharp
// 観測 API
public IReadOnlyList<BlockData> CurrentHand { get; }
public BoardData CurrentBoard { get; }  // または ReadOnly のスナップショット
public GameState CurrentState { get; }  // Playing / GameOver

// 行動 API（既存）
public bool TryPlaceBlock(BlockData blockData, BoardPosition position);
```

これは「§7 完了後の追加タスク」として ROADMAP に追記する価値があります。

---

## 進めかたの優先順位

| 優先度 | タスク | 関連 ROADMAP |
|---|---|---|
| 🔴 すぐ | `HandBlock` の Singleton 排除 | リファクタ |
| 🔴 すぐ | `CanPlaceBlock` の Debug.Log 整理 | §7 への布石 |
| 🟡 §6 着手前 | `BoardData.PlaceBlock` への責務集約設計 | §5, §6 |
| 🟡 §7 着手前 | `HandManager` の消費通知イベント設計 | §7 |
| 🟢 §7 完了後 | AI 用観測 API の追加 | 最終目標 |

§6 に進む前に、上記の小さな課題2つを片付けてから、`BoardData` の設計を考えるのが流れとして自然です。

どこから着手しますか?
