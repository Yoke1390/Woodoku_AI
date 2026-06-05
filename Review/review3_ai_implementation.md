# Woodoku AI 実装方法の比較検討（2026-06-02）

ROADMAP 最終目標「AI 環境として機能させる」に向け、**どの方式のエージェントを実装するか**を比較検討した記録。
結論を先に置き、根拠・段階計画・言語ツール選定・未解決論点を残す。

---

## 結論

- **方式: 古典的特徴量 ＋ (Noisy) CEM**（系統 B）を採用する。
- **言語/ツール: C# 内で完結**（`Woodoku.Core` をそのまま使う）。Python / ML-Agents は深層 RL に踏み込むときだけ。
- **着手の起点: 「仮想適用（look-ahead）の足場」**を作るところから。これが現状唯一の欠落ピース。

---

## 現状の土台（すでにあるもの）

強いエージェントは `IWoodokuAgent` を1個実装して差し替えるだけの状態。

| 役割 | 実体 | 状態 |
|---|---|---|
| 環境(Gym風) | [`WoodokuEnv`](../Woodoku_Unity/Assets/Script/Core/WoodokuEnv.cs)（`Reset`/`Step`/`LegalActions`、報酬=スコア差分、done=GameOver） | ✅ |
| エージェント抽象 | [`IWoodokuAgent.SelectAction(obs, legalActions)`](../Woodoku_Unity/Assets/Script/Core/Agents/IWoodokuAgent.cs) | ✅ |
| ベースライン | [`RandomAgent`](../Woodoku_Unity/Assets/Script/Core/Agents/RandomAgent.cs) | ✅ |
| 合法手の列挙 | [`GameSession.GetLegalActions()`](../Woodoku_Unity/Assets/Script/Core/GameSession.cs:58)（スロット×基準位置でフィルタ済み） | ✅ |
| 可視化実行 | [`AgentRunner`](../Woodoku_Unity/Assets/Script/Unity/AgentRunner.cs)（コルーチン、stepDelay で間引き） | ✅ |
| 純粋ロジック層 | `Woodoku.Core`（`noEngineReferences: true`、UnityEngine 非依存） | ✅ |

---

## アプローチ比較

| 系統 | 例 | 実装重量 | 強さ | 本プロジェクト適性 |
|---|---|---|---|---|
| **A. 探索** | 1手貪欲、expectimax、MCTS | 軽〜中 | 中〜高 | ◯ ベースライン向き |
| **B. 特徴量＋線形評価＋重み最適化** | **手作り特徴 + (Noisy) CEM** | 軽 | 高（Tetris系で実績） | ◎ **採用** |
| **C. 深層RL** | DQN / PPO / AlphaZero風 | 重 | 高 | △ 現時点ではオーバースペック |

### B を採用した理由

- **NN/GPU/Python 不要** — `WoodokuEnv` を C# でヘッドレスに大量試行するだけで成立する。
- **実証済み** — Szita & Lőrincz (2006) "Learning Tetris Using the Noisy CEM" が同型。
  標準 CEM は分散 σ² が早期に潰れて局所解に陥る → 各反復で σ² にノイズを加え探索を維持する改良が「Noisy」。
- **解釈可能** — 学習後の重みベクトルを見れば方策の傾向（穴を嫌う / ライン完成を好む 等）が読める。
  学習プロジェクトとして教材価値が高い。
- **既存資産と地続き** — Core が Unity 非依存なので、A → B → （必要なら）C と段階的に進める土台がそのまま使える。

---

## ⚠️ 設計上の鍵: 仮想適用（look-ahead）の足場

貪欲法も CEM も本質は「**各合法手を適用した後の盤面を採点して argmax を選ぶ**」こと。
ところが現状:

- [`GameSession.TryPlaceBlock`](../Woodoku_Unity/Assets/Script/Core/GameSession.cs:70) は**破壊的**（本物のセッションが進む）→ `SelectAction` 内では使えない。
- [`BoardSnapShot`](../Woodoku_Unity/Assets/Script/Core/Primitive/BoardSnapShot.cs) は `CellState[,]` をクローン保持するだけで、
  **「手を適用した後の盤面を返す純関数」がまだ無い**。

→ ここが実装の出発点。設計の選択肢:

| 案 | 概要 | 評価観点 |
|---|---|---|
| (a) ディープコピー＋適用 | 毎手 `BoardData` を複製して `TryPlaceBlock` | 単純。CEM の大量試行で速度が出るか要検証 |
| (b) 純関数 | `BoardSnapShot.Apply(action) → 新 SnapShot`（消去込み） | 低結合・テスト容易。`BoardData` のクリア判定ロジックを共有できる設計にできるか |
| (c) 適用→評価→巻き戻し | in-place で適用し評価後に戻す | 速いが状態管理が脆い。並行評価に弱い |

**方針メモ:** クリア判定（行/列/3×3）のロジックは `BoardData` にしか無いので、(b) を採るならそのロジックを
`BoardSnapShot` と共有できる形（static ヘルパー or Core 内の純関数）に切り出す必要がある。重複実装は避ける。

---

## 段階計画

```
Step 0: 仮想適用の足場    … 上記 (a)/(b)/(c) を決め、非破壊の「適用後盤面」を得る手段を用意
Step 1: 特徴量抽出を分離  … IFeatureExtractor : (盤面) → float[]
Step 2: 線形評価のGreedy  … IBoardEvaluator（w·features）+ 手書き重みでベースライン確立
Step 3: CEM で重み最適化  … 目的関数 J(w) = 複数シードの平均スコア
Step 4: Noisy CEM         … σ² にノイズ加算で早期収束を回避
```

### 責務分離の骨格（シグネチャのみ。実装は本人担当）

```csharp
public interface IFeatureExtractor  { float[] Extract(/* 適用後の盤面 */); }
public interface IBoardEvaluator     { float Evaluate(float[] features); }   // 線形: w·features

// CEMAgent : IWoodokuAgent
//   legalActions を 仮想適用→Extract→Evaluate で採点し argmax を返す
// CEMTrainer（Core側・ヘッドレス）
//   N(μ, σ²) から w 群をサンプル → 各 w で J(w) を評価 → 上位エリートで μ,σ を更新（+ Noisy）
```

CEM が触るのは**重みベクトル `w` だけ**。特徴量・評価・エージェントを分離しておけば、
CEM は「ブラックボックスの目的関数 `J(w)` を最適化する」役に徹せる（Strategy 的・低結合）。

### Woodoku 向け特徴量の候補（Tetris の Dellacherie / Bertsekas 特徴が下敷き）

- 埋まりセル数 / 空きセル数
- 「あと1〜2マスで揃う」行・列・3×3 ブロックの数（near-complete）
- 穴（周囲を埋められた孤立空き）の数
- 凹凸・断片化の度合い（連結性）
- 次に置ける場所の多さ（将来の自由度＝詰みにくさ）
- そのターンの消去ライン数

---

## 言語・ツールの選定

| 選択肢 | 向く方式 | 評価 |
|---|---|---|
| **C# 内で完結** | 探索・CEM | ◎ 境界をまたがず既存資産そのまま。`Woodoku.Core` が `noEngineReferences` なので、**Unity を起動せず .NET コンソール/テストから `WoodokuEnv` を回せば学習ループが桁違いに速い**（asmdef の共有方法は要検討） |
| Python ブリッジ(gRPC/socket) | 深層RL | △ Gymnasium 互換にできるが通信オーバーヘッド大。CEM には過剰 |
| Unity ML-Agents | 深層RL(PPO/SAC) | △ 標準装備だが巨大な行動空間＋合法手マスクの扱いが面倒。CEM には不要 |

CEM + 線形評価なら数値ライブラリすら不要（正規分布サンプリングと内積だけ）。→ **C# で完結が最もシンプルで速い。**

---

## 応用視点（将来）

- 評価は基本「1手読み」。手札は3つ見えているので、**3手先まで貪欲展開**して順序最適化するだけでも安価に伸びる。
- 手札補充のランダム性を真面目に扱うなら **expectimax / determinized MCTS**（手札供給をチャンスノードに）。CEM が頭打ちになってからで十分。
- CEM で得た強い方策は、後で深層 RL をやる際の**教師データ・報酬整形**に転用できる。

---

---

## 追記: 先行研究 blokie とオリジナリティの方向（2026-06-02）

### blokie (gary-z) の分析

| 項目 | 内容 |
|---|---|
| 対象 | Blockudoku / Woodoku / Block Sudoku（同型）。平均 **150万点** |
| 状態表現 | **bitboard（`uint64`×2 で81bit）**。`BitBoard` 型に row/column/cube マスク・shift・count(popcount) |
| 探索 | **「3ピースを1手」として全数探索**。pruning・深さ制限なし。`makeMoveLookahead`(4つ目考慮)は実装済みだがデフォルト未使用・効果未検証 |
| 評価関数 | "board cleanliness" = `EvalWeights` の12特徴（squashedEmpty / corneredEmpty / transition / deadlyPiece / occupiedCornerCube 等）の重み付き線形 |
| 重み最適化 | **遺伝的アルゴリズム(GA)**、C++ オフライン |
| 最適化指標 | **スコアではなく生存手数(moves)**（fitness.cpp）。「150万点」は moves からの換算 |
| 言語/ツール | C++（エンジン・GA）＋ JS/HTML（デモ）＋ Emscripten/WebAssembly |

**含意:** 当初の「特徴量＋CEM」は blokie の「cleanliness＋GA」とほぼ同型 → **そのままでは劣化再現**。
オリジナリティは別軸で出す。blokie の弱点＝**見えている3ピースしか考えず、次セットの確率分布を無視**（pruning も無し）。

### コードで判明した前提

- **手札供給**（[`HandManager`](../Woodoku_Unity/Assets/Script/Core/HandManager.cs:50)）は全種から**一様独立ランダム**、3つ使い切りで同時補充。
  → **max(現3ピース配置) → chance(次3ピース, `|shapes|³` の既知一様分布)** の expectimax 構造に綺麗に対応。
  blokie が捨てた確率を、分布既知ゆえ陽に展開できる。
- **スコア式**（[`ScoreManager`](../Woodoku_Unity/Assets/Script/Core/ScoreManager.cs:23)）は独自で blokie の150万点と直接比較不可。
  → **比較指標は「生存手数」に一本化（決定）**。blokie も生存手数を最適化しており最も正確な土俵。実ゲーム式スコアは追わない。

### オリジナリティの主軸: 「C 基盤 ＋ A 先読み」（採用）

- **C: 比較ベンチマーク基盤** — `IWoodokuAgent` に Random/Greedy/CEM/Expectimax/MCTS を差し替え、同一環境でスコア分布・手数・計算時間を比較。Unity で候補手ヒートマップ・探索ツリーを可視化（blokie に無い見せ場）。学習プロジェクトの主役。
- **A: 確率を考慮した深い先読み** — 既知の手札分布を chance node として expectimax / determinized MCTS で次セットまで読む。blokie の ablation（2ピースで98%減）を**未来方向に拡張**し、「先読み深さ × 確率考慮の寄与」を定量化する。

### 更新後の段階計画

```
Step1 bitboard 表現＋純関数の仮想適用   … Core に BitBoard 値型。クリア判定は行/列/3×3マスクのAND
Step2 Greedy(深さ1=現3ピース全数)        … blokie 相当ベースライン
Step3 線形評価＋特徴量＋CEM/GA で重み最適化
Step4 expectimax（chance node＝次3ピース期待値）  … ★blokie 超えの本体
Step5 determinized MCTS / サンプリング＋ビーム・反復深化
（横断）C: ベンチハーネス＋Unity 思考可視化
```

---

## 確定事項（2026-06-02）

- ✅ **比較指標 = 生存手数に一本化**（blokie と同じ土俵。スコアは追わない）。
- ✅ **`BoardData` 自体を bitboard 化し、UI/ゲーム用ラッパーを被せる**。クリア判定は `BitBoard` に一本化（旧 `GetCellsToClear` の HashSet スキャンは廃止）。
- ✅ **9×9 固定に割り切る**（`GridSize` 可変は捨てる）。固定ゆえ row/col/cube の27マスクを定数テーブル化できる。

## 残る論点 / 次アクション

- [ ] **Step1 着手**: `BitBoard`（`ulong _lo,_hi`、bit index = r*9+c）を実装。難所は **行7が64bit境界をまたぐ**点 → マスクを (lo,hi) ペアの定数テーブルで持てば回避（9×9固定の利点）。
- [ ] ピース配置は shift 方式（blokie流）か「各セルの bit を立てる」方式か。後者は2ワードまたぎのシフトを避けられ C# 初実装で安全。
- [ ] 安全網: 新 `BitBoard` のクリア/配置結果が既存 `BoardData` ロジックと一致することをテストで確認してから差し替え。
- [ ] expectimax の chance node: `|shapes|³` を全数展開かサンプリング近似か。
- [ ] CEM/GA の目的関数 `J(w)`（=生存手数）の試行回数。
- [ ] ヘッドレス実行を Unity 外（.NET）か EditMode テスト内か。Core の asmdef 参照方法を確認。
