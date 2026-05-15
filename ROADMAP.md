# Woodoku Unityベース開発TODOリスト

AIの環境として機能させることを前提とした、UnityでのWoodokuベース開発のTODOリストです。**内部のデータ管理と画面の描画を明確に分ける**ことを意識して進めます。

## タスクの分類

- 🎯 **機能**: 新規ゲーム機能の実装
- 🔧 **リファクタ**: 既存コードの整理・品質向上

---

## 1. プロジェクトとデータ構造の準備
- [x] 🎯 Unityで新規2Dプロジェクトを作成する。
- [x] 🎯 `BoardData` クラスを作成し、可変サイズの盤面状態を管理する（`GameSetting.GridSize` から `BoardSize = GridSize * GridSize` で算出）。
- [x] 🎯 `BlockData` を `ScriptableObject` として作成し、ブロックの形状を `Vector2Int` の配列で定義できるようにする。
- [x] 🎯 代表的なブロックの `ScriptableObject` アセットを数種類作成する。

## 2. 盤面（グリッド）の描画設定
- [x] 🎯 1マス分を表す正方形のSpriteを用意し、`Cell` プレハブを作成する。
- [x] 🎯 `Cell` プレハブに、現在の状態（空き、ブロックあり）に応じて表示を切り替えるスクリプトをアタッチする。
- [x] 🎯 `BoardUI` クラスを作成し、`Cell` プレハブを `BoardSize × BoardSize` のグリッド状に生成・配置する処理を記述する。

## 3. 手札ブロックの生成と表示
- [x] 🎯 手札ブロックのピース (`BlockPiece`) プレハブ作成。
- [x] 🎯 手札エリア (`HandArea`) の UI 構築・3スロット配置。
- [x] 🎯 `HandBlock` クラスの作成（`BlockData` から `BlockPiece` を子オブジェクトとして生成・配置）。
- [x] 🎯 `HandManager` クラスの作成（3つの手札スロット管理、`Resources.LoadAll<BlockData>` で全ブロック取得）。
- [ ] 🎯 **ブロックのランダム選出**: 現在 `HandManager.GetBlockData()` はインデックス `[3]` で固定（`// tmp` 済み）。`UnityEngine.Random.Range` で実装する。

## 4. ドラッグ＆ドロップ機能の実装（UI操作と内部ロジックの分離）
- [x] 🎯 `DraggableBlock` コンポーネントの作成（`IBeginDragHandler`, `IDragHandler`, `IEndDragHandler` 実装）。
- [x] 🎯 `OnBeginDrag` 処理:
  - スケールを 1.0f に拡大
  - 元の位置 (`initialLocalPosition`) を記憶
  - `SetAsLastSibling()` で最前面化
  - `CanvasGroup.blocksRaycasts = false` でレイキャスト無効化
- [x] 🎯 `OnDrag` 処理: 指の画面座標に追従。
  - [ ] 🎯 **Y軸方向のオフセット表示**: プレイヤーの指でブロックが隠れないように上方向に少しオフセット（未実装）。
- [x] 🎯 `OnEndDrag` 処理: 盤面座標変換 → `DropHandler` 経由で `WoodokuGameManager.HandleDropRequest` に依頼。
- [x] 🎯 配置失敗時の元の位置・スケールへの復帰 (`DraggableBlock.ResetBlock` + `HandBlock.ResetScale`)。
- [ ] 🔧 復帰アニメーション: 現在は瞬時に戻る。Tween 等で自然な動きに（任意）。

## 5. 配置判定とデータ更新
- [x] 🎯 ドロップされた基準座標から、ブロック形状の各セルを盤面座標に変換 (`BoardData.CanPlaceBlock` 内)。
- [x] 🎯 範囲内判定 (`BoardData.IsValid` / `GetCell` が無効座標で `-1` を返す)。
- [x] 🎯 空きセル判定 (`CanPlaceBlock` 内で `cellValue != 0` を確認)。
- [x] 🎯 配置時のデータ更新 (`SetCell(pos, 1)`) と UI 反映 (`CellUpdate` イベント → `BoardUI.UpdateCellState`)。
- [x] 🎯 配置失敗時の手札位置への復帰（§4 と一体）。
- [ ] 🔧 **配置〜消去ロジックの `BoardData` への集約**: 現在 `WoodokuGameManager.PlaceBlock` (private) が `SetCell` を直接呼んでいる。§6 の消去判定と一体化させるため、`BoardData.PlaceBlock(BlockData, BoardPosition) : PlacementResult` に責務移譲する設計に変更。
- [ ] 🔧 `WoodokuGameManager.GetBlockBaseBoardPosition` の薄いラッパー削除: 中身が `boardUI.TryScreenPointToBoardPosition` への単純な転送のみ。`HandleDropRequest` 内に直接書く。
- [ ] 🔧 起動時テスト用の `boardData.SetCell(0, 0, 1)` / `SetCell(2, 7, 1)` を削除 (`WoodokuGameManager.Start`)。

## 6. 消去判定（ラインと3x3ブロック）の実装

ROADMAP 方針に従い、判定ロジックは **すべて `BoardData` に集約**する。UI 反映は既存の `CellUpdate` イベント経由で自動的に走るため、UI 側の改修は不要。

### 6.1 判定ロジック
- [ ] 🎯 **行の判定**: 各行 (Y=0〜BoardSize-1) の全マスが `1` であるか確認し、消去対象リストに追加。
- [ ] 🎯 **列の判定**: 各列 (X=0〜BoardSize-1) の全マスが `1` であるか確認し、消去対象リストに追加。
- [ ] 🎯 **3x3エリアの判定**: 盤面を `GridSize × GridSize` のサブブロックに分け、各サブブロック全マスが `1` であるか確認し、消去対象リストに追加。
- [ ] 🎯 重複セルの統合（行・列・3x3 が同時成立する場合）。

### 6.2 配置〜消去の統合
- [ ] 🎯 `BoardData.PlaceBlock(BlockData, BoardPosition) : PlacementResult` の実装（配置 → スキャン → クリアを一気通貫）。
- [ ] 🎯 結果オブジェクト `PlacementResult` の定義:
  - `Success` (bool)
  - `ClearedLines` (IReadOnlyList<...>): 後のスコア計算・エフェクト用
- [ ] 🎯 消去対象セルの `SetCell(pos, 0)` で `CellUpdate` イベント発火 → UI が自動的に空きセル表示に切り替わることを確認。

### 6.3 関連リファクタ
- [ ] 🔧 `BoardData.CanPlaceBlock` 内の `Debug.Log` を削除または `#if UNITY_EDITOR` で囲む（§7 の手詰まり判定で大量呼び出しのため）。

## 7. ゲームサイクルとゲームオーバー判定

### 7.1 手札の消費と補充
- [ ] 🎯 手札ブロック消費の通知経路追加:
  - 現状 `DraggableBlock.OnEndDrag` 成功時に `Destroy(gameObject)` するだけで `HandManager` は知らない。
  - `DropHandler` とは別の `Action<int slotIndex>` 等で `HandManager` に通知する。
- [ ] 🎯 `HandManager` に消費カウント・残りスロット管理を追加。
- [ ] 🎯 `HandManager.HandEmpty` イベント（または同等の通知）を追加。
- [ ] 🎯 全スロット消費時に、`HandManager` が新たに3つのブロックを補充。

### 7.2 手詰まり判定
- [ ] 🎯 タイミング: ブロック補充直後 / 配置直後の両方で実行。
- [ ] 🎯 判定ロジック: 手札の各 `BlockData` について、盤面全座標 (0,0)〜(BoardSize-1, BoardSize-1) で `CanPlaceBlock` を試す全探索。
- [ ] 🎯 判定 API: `WoodokuGameManager.HasAnyValidPlacement(IEnumerable<BlockData>) : bool`。
- [ ] 🎯 1箇所でも置ければゲーム続行、どこにも置けなければゲームオーバー。

### 7.3 ゲームオーバー処理
- [ ] 🎯 ゲーム状態の管理: `enum GameState { Playing, GameOver }`。
- [ ] 🎯 ゲームオーバー UI の表示。
- [ ] 🎯 リスタート機能: `BoardData.Reset()` + 手札再生成 + 状態リセット。

## 8. AI 連携用 API の整備

ROADMAP 冒頭の「AI環境として機能させる」最終目標に向けた、外部から状態を観測し行動を入力できる API。

### 8.1 観測 API
- [ ] 🎯 `WoodokuGameManager.CurrentHand : IReadOnlyList<BlockData>` — 現在の手札。
- [ ] 🎯 `WoodokuGameManager.CurrentBoard` — 読み取り専用の盤面スナップショット（`int[,]` のコピー、または専用 ReadOnly 型）。
- [ ] 🎯 `WoodokuGameManager.CurrentState : GameState` — 現在のゲーム状態。

### 8.2 行動 API
- [x] 🎯 `WoodokuGameManager.TryPlaceBlock(BlockData, BoardPosition) : bool` — 既存。AI もこれを呼ぶ想定。

### 8.3 イベント
- [ ] 🎯 `WoodokuGameManager.StateChanged` — Playing / GameOver の遷移通知（AI のエピソード終端検知用）。
- [ ] 🎯 配置・消去・補充の各イベント（学習信号として有用）。

---

## リファクタリング: 横断的な改善

機能実装と並行して、もしくは合間に行う構造改善。優先度順。

### Singleton 依存の整理
- [ ] 🔧 **`HandBlock` の `BoardUI.Instance` 依存排除**:
  - 現状: `HandBlock.Initialize()` 内で `BoardUI.Instance.CellSize` を参照。
  - 対応: `Initialize(BlockData, float cellSize)` で外から渡す。`HandManager` 側も `boardUI` 参照を持ち、生成時に渡す。
- [ ] 🔧 **`WoodokuGameManager.Instance` Singleton の整理**: 現在のコードから外部参照がなくなっている → 削除を検討。
- [ ] 🔧 **`BoardUI.Instance` Singleton の整理**: `HandBlock` の依存排除後、削除可能。

### API 設計の見直し
- [ ] 🔧 `BoardUI.TryScreenPointToBoardPosition` の失敗時 `out` 値 `(-1, -1)` のセンチネル: `default(BoardPosition)` か明示的な無効状態に変更を検討。
- [ ] 🔧 `BoardData.SetCell` の `int value` パラメータ: `0`/`1` のマジックナンバー → `enum CellState { Empty, Filled }` への変更を検討（消去・特殊タイル等の将来拡張に備える）。

### コード品質
- [ ] 🔧 `BoardData.CanPlaceBlock` 内の `Debug.Log` 整理（§6.3 と同じ）。
- [ ] 🔧 起動時テスト用 `SetCell` の削除（§5 リファクタと同じ）。
- [ ] 🔧 `BoardUI.BoradData_OnCellUpdate` のタイポ修正（`Borad` → `Board`）。
