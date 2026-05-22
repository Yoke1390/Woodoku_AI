# BoardPosition / BlockOffset リファクタリング TODO（方針A: ピュアな値型）

## 設計方針

`BoardPosition` を「盤面サイズに依存しない、ピュアな `(x, y)` 値型」に変更する。
境界チェックの責務は `BoardData` に一本化する。

### 判断理由

- 座標の有効性は盤面サイズ（外部コンテキスト）に依存する → 値自身がバリデーションを持つ必要がない
- §6 消去判定・§7 手詰まり全探索で大量生成される → ファクトリの冗長さがコストになる
- `BoardData` が既にゲート（`GetCell` / `SetCell` / `CanPlaceBlock`）を持っている → 二重チェック不要

---

## TODO

### Phase 1: `BoardPosition` のスリム化

- [x] `BoardPosition` から `_boardSize` フィールドを削除
- [x] `BoardPosition` から `IsValid` プロパティを削除
- [x] コンストラクタを `(int x, int y)` のみに変更（バリデーションなし）
- [x] `BoardPosition.IsInBoard` static メソッドを削除（`BoardData` に移す）
- [x] `BoardPosition.TryAdd` static メソッドを削除（`BoardData` に移す）

### Phase 2: `BoardData` へ責務を移動

- [x] `BoardData.TryOffset(BoardPosition pos, BlockOffset offset, out BoardPosition result)` を追加
  - 旧 `BoardPosition.TryAdd` の処理を移す。`BoardSize` は `BoardData` が知っている
- [x] `BoardData.IsInBoard(int x, int y)` を追加（または既存の `IsValid` をリネーム）
- [x] `BoardData.IsValid(BoardPosition)` → `BoardData.IsInBoard(BoardPosition)` にリネーム
  - 「Valid」だと BoardPosition 自身の妥当性に聞こえる。盤面内かどうかなので `IsInBoard` が適切
- [x] `BoardData.MakeBoardPosition(int x, int y)` を削除（コンストラクタが `(x, y)` だけなので不要）
- [x] `CanPlaceBlock` 内の `BoardPosition.TryAdd` 呼び出しを `this.TryOffset` に変更

### Phase 3: 利用箇所の修正

- [x] [BoardUI.cs L112](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/BoardUI.cs#L112): `new BoardPosition(boardPositionX, boardPositionY)` — 引数が2つになるだけで変更不要（確認のみ）
- [x] [BoardUI.cs L116](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/BoardUI.cs#L116): `new BoardPosition(-1, -1)` → `default(BoardPosition)` に変更
  - 失敗時の値は `TryXxx` パターンの `bool` で判別するので、値自体は何でもよい
- [x] [WoodokuGameManager.cs L66-68](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/WoodokuGameManager.cs#L66-L68): `PlaceBlock` 内の `blockBaseBoardPosition + blockPiecePosition`
  - 現状コンパイルエラーのはず。`TryOffset` を使うか、`CanPlaceBlock` 通過済みなら安全に足せる `Add` メソッドを用意するか検討

### Phase 4: コンパイルエラーの解消（既存の型混同）

> [!WARNING]
> 以下はリファクタ以前から存在するコンパイルエラーの可能性。先に確認すること。

- [ ] [HandBlock.cs L32](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/HandBlock.cs#L32): `BoardPosition blockPosition = blockData.BlockCells[i]` → `BlockOffset` で受ける
- [ ] [HandBlock.cs L37](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/HandBlock.cs#L37): `blockPosition - BlockData.Center` → 演算子定義が必要か、`Vector2` に変換して計算
- [ ] [WoodokuGameManager.cs L66](file:///Users/yosukemaeda/Code/Toy/Woodoku_AI/Woodoku_Unity/Assets/Script/WoodokuGameManager.cs#L66): `foreach (BoardPosition blockPiecePosition in blockData.BlockCells)` → `BlockOffset` で受ける

### Phase 5: 演算子の整理

- [ ] `BoardPosition` に演算子を追加するか方針を決める
  - **推奨**: `BoardPosition + BlockOffset` → `BoardPosition` の演算子を追加。ただしこれは境界チェックなしなので、`CanPlaceBlock` 通過後の「安全な場面」でのみ使う想定
  - 代替: 演算子は一切定義せず、すべて `BoardData.TryOffset` 経由に統一する（安全だがやや冗長）
- [x] `BlockOffset` の `explicit operator` (`Vector2Int` → `BlockOffset`) はそのまま維持

---

## 完了後の姿

```csharp
// BoardPosition — ピュアな値
public readonly struct BoardPosition
{
    public int x { get; }
    public int y { get; }
    public BoardPosition(int x, int y) { ... }
    // 演算子（採用する場合）
    public static BoardPosition operator +(BoardPosition pos, BlockOffset offset) { ... }
}

// BlockOffset — 変更なし
public readonly struct BlockOffset { ... }

// BoardData — 境界チェックの唯一の責務者
public class BoardData
{
    public bool IsInBoard(BoardPosition pos) { ... }
    public bool IsInBoard(int x, int y) { ... }
    public bool TryOffset(BoardPosition pos, BlockOffset offset, out BoardPosition result) { ... }
    // GetCell, SetCell, CanPlaceBlock は引き続きここ
}
```
