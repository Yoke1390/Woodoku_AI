# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Working mode (important)

`.agents/rules/learner-guide.md` defines how to interact here, and it overrides the
usual "just implement it" default:

- This is a **learning project**. The user writes the code; you act as a tech-lead/mentor.
- **Do not write full implementations unless the user explicitly asks.** Otherwise stay at
  the level of hints, class signatures, or skeleton code.
- Lead with design reasoning (OOP, SOLID, GoF patterns, low coupling, composition-over-
  inheritance). Be wary of Singleton overuse in Unity.
- Respond in Japanese, concise senior-engineer tone: 結論 (short verdict) → 現状の分析 →
  解決へのヒント → 応用視点 (only when genuinely useful).

`ROADMAP.md` (root, Japanese) is the source of truth for project scope and task status.
Read it before proposing work — `[x]` = done, `[ ]` = pending, `🎯` = feature, `🔧` = refactor.

## Project

A Woodoku (block-puzzle sudoku) game built in Unity, intended ultimately as an **environment
for an AI agent** to observe state and take actions. The Unity project lives in `Woodoku_Unity/`.

- Unity Editor **2022.3.10f1**, C# 9, .NET Standard 2.1.
- No build/test scripts and no test suite exist. Develop by opening `Woodoku_Unity/` in the
  Unity Editor and pressing Play. `Assets/Scenes/MainScene.unity` = human play;
  `Assets/Scenes/AgentRunner.unity` = an agent plays automatically.
- All gameplay code is under `Woodoku_Unity/Assets/Script/`. `Library/`, `Temp/`, `obj/` are
  generated — ignore them.

## Architecture

Two assemblies enforce the central principle — a strict split between **data/logic** and
**rendering** — at the *compiler* level. `Woodoku.Core` (asmdef, `noEngineReferences: true`)
holds all game state/logic and may not reference `UnityEngine`; `Woodoku.Unity` depends on Core
and does rendering/input. Dependency is one-way (Unity → Core), so writing `using UnityEngine`
in the logic layer is a compile error.

**Data / logic — `Assets/Script/Core/` (no UnityEngine):**
- `GameSession` — the game proper and the **only path that mutates state**. Wraps `BoardData` +
  `HandManager` + `ScoreManager` (all private) and exposes them as read-only interfaces
  (`IReadOnlyBoard` / `IReadOnlyHands` / `IReadOnlyScore`). `TryPlaceBlock(slot, pos)` and
  `TryPlaceBlock(AgentAction)` place a block; `State : GameState { Playing, GameOver }`; raises
  `GameOver` when the hand has no legal action left.
- `BoardData` — owns the `CellState[,]` grid (sized from `GameSetting.GridSize`:
  `BoardSize = GridSize*GridSize`, default 9×9, `NGrids` sub-blocks/axis). Holds *all* placement
  and line-clear logic: `TryPlaceBlock` (place → scan rows/cols/3×3 → clear) and
  `EnumerateLegalActions(BlockShape)`. Emits `CellUpdate` on every cell change — the only channel
  the UI listens to.
- `HandManager` — owns the N (=`WoodokuGameManager.NHandSlots`, 3) hand slots, picks shapes with
  a seeded `System.Random`, refills when empty; raises `HandBlockGenerated` /
  `HandBlockConsumed` / `HandSettled`.
- `ScoreManager` — score from block size + line-clear/combo/streak bonuses; raises `ScoreUpdate`.
- `BlockShape` — the pure-logic piece type. The `BlockData` `ScriptableObject` (`Unity/Hand/`,
  `Vector2Int[]`, bulk-loaded with `Resources.LoadAll<BlockData>("")`) is authoring-only;
  `BlockData.ToShape()` is the boundary. `BoardPosition` / `BlockOffset` / `AgentAction` /
  `PlacementAction` / `PlacementResult` are `readonly struct` value types in `Core/Primitive/`.

**AI environment — `Assets/Script/Core/` (also Unity-free):**
- `WoodokuEnv` — Gym-like wrapper over `GameSession`: `Reset(seed)`, `Step(AgentAction)`
  (`reward` = score delta, `done` = GameOver), `LegalActions`. `Observation` (board+hands) and
  `StepResult` are `readonly struct`s.
- `IWoodokuAgent.SelectAction(Observation, legalActions)` with baseline `RandomAgent`
  (`Core/Agents/`).

**UI / Unity — `Assets/Script/Unity/`:**
- `WoodokuGameManager` — human-play orchestrator. `Initialize` loads `BlockData` from Resources,
  builds the `GameSession`, and wires `gameSession.Board/Hands/Score` into `BoardUI` / `HandUI` /
  `ScoreUI`. Input arrives via the `EndBlockMoveHandler` delegate (`HandleEndBlockMoveRequest`):
  screen→board conversion, then `GameSession.TryPlaceBlock`.
- `AgentRunner` — same wiring, but drives the session from an `IWoodokuAgent` in a coroutine with
  human input disabled (`Assets/Scenes/AgentRunner.unity`).
- `BoardUI` — instantiates `Cell` prefabs into a `GridLayoutGroup`, computes `CellSize`, converts
  screen→board (`TryScreenPointToBoardPosition`). Passive: redraws cells only on `CellUpdate`.
- Hand input — `BlockManipulator` holds the move state (`BeginMove` / `EndMove` / follow-pointer);
  `DragBlockControlInput` and `ClickBlockControlInput` are interchangeable input front-ends
  selected by `GameSetting.BlockControlMode` (Drag / Click). On drop they call the
  `EndBlockMoveHandler` delegate, keeping input decoupled from board logic.

**Placement flow:** input → `BlockManipulator.EndMove` → `EndBlockMoveHandler`
(`WoodokuGameManager.HandleEndBlockMoveRequest`) → `BoardUI` screen-to-board conversion →
`GameSession.TryPlaceBlock` → `CellUpdate` / `ScoreUpdate` events → UI redraw.

## Ongoing redesign — Functional Core / Imperative Shell (active work)

The data/logic layer above is **mid-migration**. The current code still has the mutable
`BoardData` / `HandManager` / `ScoreManager`, but the agreed target (ROADMAP §9, full plan with
diagrams in `Review/review4_functional_core_redesign.md`) collapses them into:

- **One immutable state** `GameStateData` (board + hand + streak + score + `Rng` + `GameStatus`),
  with `Hand` and `Rng` (functional RNG replacing `System.Random`) as immutable value types, and
  `RuleSet` (grid size / slot count / shape pool) as immutable *config* (not state).
- **One pure transition** `GameRules`, split into the deterministic `ApplyPlacement(state, action)`
  (board place+clear → score+streak → hand-slot consume) and the stochastic
  `RefillIfNeeded(state, ruleSet)` (draws from `Rng` only when the hand empties). `BoardRule` /
  `ScoreRule` / `BoardSimulator` survive as the pure leaf calculators it composes.
- **A thin shell** `GameSession` holding a single `GameStateData _state`, whose `TryPlaceBlock`
  runs the pure transition then **reconstructs events from the old→new state diff**
  (`RaiseDiffEvents`) — the one and only event source. Event signatures stay the same, so the UI
  is untouched.

The point: **human play and AI look-ahead call the same `GameRules.ApplyPlacement`** (DRY). The AI
never receives `Rng`, so it cannot peek at future pieces — refill is the only stochastic seam.
When editing Core, prefer the target shape; don't add new logic to the soon-to-be-deleted managers.

## Notable in-progress state

- **Core redesign (ROADMAP §9, `Review/review4_*`) is the active task** — unifying human play and
  AI simulation onto one immutable state + one pure transition (see the section above). This builds
  the permanent look-ahead foundation the strong agent needs.
- The strong AI agent is the main remaining feature. The environment harness (`WoodokuEnv`,
  `IWoodokuAgent`, `RandomAgent`, `AgentRunner`) is built, but a competitive agent (features +
  linear eval + (Noisy) CEM, then probability-aware expectimax/MCTS) is not. The plan lives in
  `Review/review3_ai_implementation.md` and ROADMAP §8.4.
- Seeding: `GameSession` defaults to `seed = TestSeed (1234)`; `Begin(seed)` /
  `WoodokuEnv.Reset(seed)` take an explicit seed, but `WoodokuGameManager` still uses the default
  (ROADMAP §5).
- `README.md` (Japanese) is the recruiter-facing entry point for the portfolio; screenshots/GIFs
  are still TODO.
