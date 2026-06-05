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

## 2.5 盤面のビジュアルデザイン

グリッドを画像のような「木製棚の凹み」スタイルにする。プレハブとスクリプトを組み合わせ、**グリッドサイズに応じて動的に生成**する。

### 設計方針

- 入れ子構造は使わず、`GridLayoutGroup` による現構造を維持する。
- `Cell` プレハブを2レイヤー化し、セルの色と境界線を分離する。
- 境界線は **右辺・上辺のみ** を各セルが持つ。左辺・下辺は隣接セルの右辺・上辺で賄う。
- `BoardUI.Initialize()` 内で各セルの座標を計算し、スタイルを注入する（`Cell` は受動的にスタイルを受け取るだけ）。

### Cellプレハブの構成

```
Cell (RectTransform)
  ├── EmptyBackground (Image)   ← 常時表示。空きセルの色（暗い赤褐色）
  ├── FilledOverlay   (Image)   ← Show/Hide。配置ブロックの色
  ├── BorderRight     (Image)   ← 右辺境界線（Anchor: 右端 stretched）
  └── BorderTop       (Image)   ← 上辺境界線（Anchor: 上端 stretched）
```

### 境界線の表示ルール（BoardUI が初期化時に注入）

| 条件                                                                 | BorderRight / BorderTop |
| -------------------------------------------------------------------- | ----------------------- |
| 外枠（`x == BoardSize-1` / `y == BoardSize-1`）                      | **非表示**              |
| 3x3グループ境界（`(x+1) % GridSize == 0` / `(y+1) % GridSize == 0`） | **太い・濃い色**        |
| それ以外                                                             | **細い・薄い色**        |

### タスク

- [x] 🎯 `Cell` プレハブに `EmptyBackground`・`FilledOverlay`・`BorderRight`・`BorderTop` の4つの Image を追加する。
- [x] 🎯 `Cell.cs` に `SetBorderRight(Color, float)`・`HideBorderRight()`・`SetBorderTop(Color, float)`・`HideBorderTop()` を実装する。
- [x] 🎯 `Cell.cs` に `SetBackgroundColor(Color)` を実装する。
- [x] 🎯 `BoardUI.InitializeCells()` にスタイル注入ロジックを追加する（座標計算 → 外枠/3x3境界/通常の3ケース分岐）。
- [x] 🎯 `BoardUI` にスタイル設定用のSerializeFieldを追加する（`normalBorderColor`, `groupBorderColor`, `normalBorderWidth`, `groupBorderWidth`, `emptyCellColor`）。

---

## 3. 手札ブロックの生成と表示

- [x] 🎯 手札ブロックのピース (`BlockPiece`) プレハブ作成。
- [x] 🎯 手札エリア (`HandArea`) の UI 構築・3スロット配置。
- [x] 🎯 `HandBlock` クラスの作成（`BlockData` から `BlockPiece` を子オブジェクトとして生成・配置）。
- [x] 🎯 `HandManager` クラスの作成（3つの手札スロット管理、`Resources.LoadAll<BlockData>` で全ブロック取得）。
- [x] 🎯 **ブロックのランダム選出**: `HandManager.GetRandomBlockShape()` が `System.Random` で実装済み（コンストラクタの `randomSeed` で再現可能）。ロジック層は `BlockData`(ScriptableObject) ではなく純粋型 `BlockShape` を扱う。

## 4. ドラッグ＆ドロップ機能の実装（UI操作と内部ロジックの分離）

> 📝 **その後リファクタ済み（入力と移動の分離）**: `DraggableBlock` は廃止。移動状態を持つ `BlockManipulator` と、入力デバイス別の `DragBlockControlInput` / `ClickBlockControlInput` に分割し、`GameSetting.BlockControlMode`（Drag / Click）で切り替える。配置依頼は `DropHandler` ではなく `EndBlockMoveHandler` デリゲート経由（`WoodokuGameManager.HandleEndBlockMoveRequest`）。以下は当時の `DraggableBlock` 前提の記述。

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
- [x] 🔧 **配置〜消去ロジックの `BoardData` への集約**: 現在 `WoodokuGameManager.PlaceBlock` (private) が `SetCell` を直接呼んでいる。§6 の消去判定と一体化させるため、`BoardData.PlaceBlock(BlockData, BoardPosition) : PlacementResult` に責務移譲する設計に変更。
- [x] 🔧 `WoodokuGameManager.GetBlockBaseBoardPosition` の薄いラッパー削除: 中身が `boardUI.TryScreenPointToBoardPosition` への単純な転送のみ。`HandleDropRequest` 内に直接書く。
- [x] 🔧 起動時テスト用の `SetCell` 呼び出しは削除済み (`WoodokuGameManager`)。
- [ ] 🔧 ハードコードされた乱数シードの整理: 現在は `GameSession` の既定引数 `seed = TestSeed (1234)`。`GameSession.Begin(seed)` / `WoodokuEnv.Reset(seed)` で注入は可能だが、`WoodokuGameManager` は既定シードのまま。本番投入時にランダム化 or 注入経路を整理。

## 6. 消去判定（ラインと3x3ブロック）の実装

ROADMAP 方針に従い、判定ロジックは **すべて `BoardData` に集約**する。UI 反映は既存の `CellUpdate` イベント経由で自動的に走るため、UI 側の改修は不要。

### 6.1 判定ロジック

- [x] 🎯 **行の判定**: 各行 (Y=0〜BoardSize-1) の全マスが `1` であるか確認し、消去対象リストに追加。
- [x] 🎯 **列の判定**: 各列 (X=0〜BoardSize-1) の全マスが `1` であるか確認し、消去対象リストに追加。
- [x] 🎯 **3x3エリアの判定**: 盤面を `GridSize × GridSize` のサブブロックに分け、各サブブロック全マスが `1` であるか確認し、消去対象リストに追加。
- [x] 🎯 重複セルの統合（行・列・3x3 が同時成立する場合）。

### 6.2 配置〜消去の統合

- [x] 🎯 `BoardData.TryPlaceBlock(BlockShape, BoardPosition) : PlacementResult` の実装（配置 → スキャン → クリアを一気通貫）。
- [x] 🎯 結果オブジェクト `PlacementResult`（`readonly struct`）の定義:
  - `IsSuccess` (bool)
  - `NClearedTimes` (int) / `ClearedCells` (IReadOnlyList<BoardPosition>): スコア計算・エフェクト用
  - 失敗用ファクトリ `PlacementResult.Failure`。
- [x] 🎯 消去対象セルの `SetCell(pos, 0)` で `CellUpdate` イベント発火 → UI が自動的に空きセル表示に切り替わることを確認。

## 7. ゲームサイクルとゲームオーバー判定

### 7.1 手札の消費と補充

- [x] 🎯 手札ブロック消費の通知経路: `DropHandler` に `slotIndex` を載せ、配置成功時に `GameSession.TryPlaceBlock(slotIndex, pos)` が内部で `HandManager.CommitPlacement(slotIndex)` を呼ぶ（盤面更新と手札消費が原子的）。
- [x] 🎯 `HandManager` に消費カウント・残りスロット管理を追加。
- [x] 🎯 `HandManager.HandEmpty` イベント（または同等の通知）を追加。
- [x] 🎯 全スロット消費時に、`HandManager` が新たに3つのブロックを補充。

### 7.2 手詰まり判定

- [x] 🎯 タイミング: ブロック補充直後 / 配置直後の両方で実行。
- [x] 🎯 判定ロジック: 手札の各 `BlockShape` について `BoardData.CanPlaceBlockInBoard`（盤面全座標を試す全探索）。
- [x] 🎯 判定 API: `GameSession.IsGameOver()` を `HandSettled` 契機で実行し、成立時に `GameOver` イベント発火。
- [x] 🎯 1箇所でも置ければゲーム続行、どこにも置けなければゲームオーバー。

### 7.3 ゲームオーバー処理

> ✅ 完了。`GameSession.GameOver` イベント（手詰まり時に発火）に加え、`GameState` enum・ゲームオーバー UI（`GameOverUI`）・リスタート（`GameSession.Begin` で盤面/手札/スコアをリセット）まで実装済み。

- [x] 🎯 ゲーム状態の管理: `enum GameState { Playing, GameOver }`。
- [x] 🎯 ゲームオーバー UI の表示。
- [x] 🎯 リスタート機能: `BoardData.Reset()` + 手札再生成 + 状態リセット。

### 7.4 スコアリング

- [x] 🎯 `ScoreManager`（純粋ロジック、`IReadOnlyScore` 実装）: 配置ブロックのマス数 + ライン消去ボーナス（同時消し数 `NClearedTimes`・連続消し `streak`）でスコア加算。`PlacementResult` を受けて計算。
- [x] 🎯 `GameSession.Score : IReadOnlyScore`（`Score` + `ScoreUpdate` イベント）で公開。UI 側は `ScoreUI` がイベント購読して表示。

## 8. AI 連携用 API の整備

ROADMAP 冒頭の「AI環境として機能させる」最終目標に向けた、外部から状態を観測し行動を入力できる API。

### 8.1 観測 API

- [x] 🎯 手札の観測: `GameSession.Hands : IReadOnlyHands`（`CurrentHand : IReadOnlyList<BlockShape?>` + `HandBlockGenerated`）。
- [x] 🎯 盤面の観測: `GameSession.Board : IReadOnlyBoard`（`GetCell(BoardPosition)` + `GridSize`/`BoardSize`/`NGrids` + `CellUpdate`）。専用の読み取り専用インターフェースで公開済み。
- [x] 🎯 ゲーム状態の観測: `GameSession.State : GameState`（`enum { Playing, GameOver }`）。`WoodokuEnv.Step` の `done` 判定にも使用。

### 8.2 行動 API

- [x] 🎯 行動 API: `GameSession.TryPlaceBlock(int slotIndex, BoardPosition) : PlacementResult` を実装済み。行動空間は「手札スロット番号 × 盤面基準位置」。空スロット等の無効手は例外ではなく `PlacementResult.Failure` を返す。
- [x] 🔧 範囲外 `slotIndex` のガード: 現状 `CurrentHand[slotIndex]` の配列アクセスで例外が飛ぶ。空スロットと同様 `Failure` に統一する。

### 8.3 イベント

- [x] 🎯 ゲームオーバー通知: `GameSession.GameOver` イベント（エピソード終端検知用）。
- [ ] 🎯 `StateChanged` — Playing ↔ GameOver の双方向遷移通知。`GameState` は導入済みなので、あとは公開イベントを足すだけ。
- [ ] 🎯 配置・消去・補充の各イベントを `GameSession` の公開面に出す（学習信号用。ロジック層には `CellUpdate`/`HandSettled`/`HandBlockGenerated` が既に存在）。

### 8.4 エージェント実行基盤（環境ラッパーと差し替え可能なエージェント）

§8.1〜8.3 の観測/行動/イベントを、強化学習でおなじみの環境インターフェースとして束ねた層。

- [x] 🎯 合法手の列挙: `BoardData.EnumerateLegalActions(BlockShape) : IEnumerable<PlacementAction>`（盤面全座標を試し、配置可能な基準位置を列挙）と、スロット横断でまとめる `GameSession.GetLegalActions() : IEnumerable<AgentAction>`。
- [x] 🎯 行動の値型 `AgentAction`（`SlotIndex` × `BasePosition`）。`GameSession.TryPlaceBlock(AgentAction)` オーバーロードあり。
- [x] 🎯 Gym 風環境ラッパー `WoodokuEnv`: `Reset(seed) : Observation` / `Step(AgentAction) : StepResult`（`reward = スコア差分`, `done = GameOver`）/ `LegalActions`。`Observation`（盤面+手札）・`StepResult`（観測+報酬+done）は `readonly struct`。
- [x] 🎯 エージェント抽象 `IWoodokuAgent.SelectAction(Observation, legalActions) : AgentAction` と、ベースライン `RandomAgent`（合法手から一様ランダム）。
- [x] 🎯 可視化実行 `AgentRunner`(MonoBehaviour): 人間入力を無効化し、コルーチンで `stepDelay` 間引きしながらエージェントの自動プレイを既存 UI に描画。シーンは `Assets/Scenes/AgentRunner.unity`。
- [ ] 🎯 強いエージェント本体（特徴量＋線形評価、(Noisy) CEM、確率を考慮した expectimax / MCTS）。方針は `Review/review3_ai_implementation.md`。

---

## リファクタリング: 横断的な改善

機能実装と並行して、もしくは合間に行う構造改善。優先度順。

### ロジック層とUI層の分離（最重要・ほぼ完了）

ROADMAP 冒頭の「データ管理と描画を明確に分ける」を、規律ではなく**コンパイラ強制**まで到達させた一連の作業。

- [x] 🔧 **値型から UnityEngine を排除**: `BoardPosition` / `BlockOffset` の `Vector2Int` ラップを廃し、素の `int x, y` に。Unity 依存の変換は UI 側の拡張メソッド（`UnityExtensions`）へ分離。
- [x] 🔧 **ロジック用の純粋型 `BlockShape` を分離**: `BlockData`(ScriptableObject) はオーサリング/インポート専用、ゲームロジックは `BlockShape` を扱う。`BlockData.ToShape()` が境界変換。
- [x] 🔧 **ドメイン型のトップレベル化**: `CellState` / `CellUpdateData` を `BoardData` のネストから独立した Core 型へ。
- [x] 🔧 **asmdef 分割**: `Woodoku.Core`（`noEngineReferences: true` で UnityEngine 参照禁止）と `Woodoku.Unity`（Core を参照）。依存は Unity → Core の一方向で、論理層に `using UnityEngine` を書くとコンパイルエラーになる。
- [x] 🎯 **`GameSession` 抽出**: `BoardData` + `HandManager` を束ねる純粋なゲーム本体。`WoodokuGameManager`(MonoBehaviour) は Resources 読込・UI 配線・画面↔盤面変換だけの薄いアダプタに。
- [x] 🔧 **読み取り専用インターフェース**: `IReadOnlyBoard` / `IReadOnlyHands` を UI に渡し、`GameSession` 内部の可変な `BoardData`/`HandManager` は private 化。盤面変更の経路は `GameSession.TryPlaceBlock` に一本化。

### Singleton 依存の整理

- [x] 🔧 **`HandBlock` の `BoardUI.Instance` 依存排除**: `HandBlock.Initialize(BlockData, float cellSize)` で `cellSize` を外から受け取る形に変更済み。`HandManager` が `Initialize` 時に受け取った `_cellSize` を渡している。
- [x] 🔧 **`WoodokuGameManager.Instance` Singleton の整理**: `Instance` は削除済み（コードベース全体に `Instance` 参照なし）。
- [x] 🔧 **`BoardUI.Instance` Singleton の整理**: `Instance` は削除済み。

### API 設計の見直し

- [x] 🔧 `BoardUI.TryScreenPointToBoardPosition` の失敗時 `out` 値: `default(BoardPosition)` を返す形に変更済み（`(-1, -1)` センチネルを廃止）。
- [x] 🔧 `BoardData.SetCell` の値パラメータ: `enum CellState { Empty, Filled, OutOfBoard }` を導入し、`SetCell(..., CellState state)` に変更済み。

### コード品質

- [x] 🔧 `BoardData.CanPlaceBlock` 内の `Debug.Log` 整理: 現コードに `Debug.Log` は残っていない。
- [x] 🔧 起動時テスト用 `SetCell` の削除済み（§5 と同じ）。
- [x] 🔧 `BoardUI.BoardData_OnCellUpdate` のタイポ修正済み（`Borad` → `Board`）。
- [ ] 🔧 `PlacementResult.Failure` の `BlockShape` が `default`（`_blocks == null` の無効値）になる問題: 現状 `IsSuccess` しか読まれず無害だが、`BlockShape` を消費する前に整理（フィールド自体を YAGNI で持たない等）。
