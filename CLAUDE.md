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
  Unity Editor and pressing Play; the main scene is `Assets/Scenes/MainScene.unity`.
- All gameplay code is under `Woodoku_Unity/Assets/Script/`. `Library/`, `Temp/`, `obj/` are
  generated — ignore them.

## Architecture

The central design principle (from ROADMAP) is a strict split between **data/logic** and
**rendering**. Pure-C# data classes hold game state and never touch Unity rendering; UI
`MonoBehaviour`s react to events.

**Data / logic (no rendering):**
- `BoardData` (`Board/`) — owns the `CellState[,]` grid. Sized from `GameSetting.GridSize`:
  `BoardSize = GridSize * GridSize` (default 9×9), with `NGrids` sub-blocks per axis. Holds
  *all* placement and line-clear logic: `CanPlaceBlock`, `TryPlaceBlock` →
  `PlaceBlockAndClear` (place → scan rows/cols/3×3 via `GetCellsToClear` → clear). Emits
  `CellUpdate` events on every cell change — this is the only channel the UI listens to.
- `BlockData` (`Block/`) — `ScriptableObject` defining a piece shape as `Vector2Int[]`. Assets
  live in `Assets/Resources/` and are bulk-loaded with `Resources.LoadAll<BlockData>("")`.
  `OnValidate` auto-normalizes coordinates so the shape's origin is `(0,0)`; derived values
  (`BlockCells`, `Center`, `MaxX/Y`) are eagerly cached.
- `BoardPosition`, `BlockOffset` — `readonly struct` value types; `BoardPosition + BlockOffset`
  is defined so block-cell-to-board math reads naturally.
- `PlacementResult` — outcome of a placement (success, cleared cells/count) for scoring/effects.

**UI / Unity:**
- `WoodokuGameManager` — root orchestrator (`Start`). Wires `BoardData` ↔ `BoardUI` (subscribes
  `BoardUI.BoradData_OnCellUpdate` to `boardData.CellUpdate`), and `HandManager` events to
  game-over checking.
- `BoardUI` — instantiates `Cell` prefabs into a `GridLayoutGroup`, computes `CellSize`,
  converts screen→board coordinates (`TryScreenPointToBoardPosition`). Passive: redraws cells
  only in response to `CellUpdate`.
- `HandManager` — owns the 3 hand slots, picks blocks (seeded `Unity.Mathematics.Random`),
  spawns `HandBlock`s, refills when empty, and raises `BlockPlaced` / `HandBlockGenerated`.
- `DraggableBlock` / `HandBlock` / `BlockPiece` / `BlockPreview` — drag-and-drop input. On drop,
  `DraggableBlock` calls a `DropHandler` delegate (the `WoodokuGameManager.HandleDropRequest`
  pointer→`BoardData.TryPlaceBlock` bridge), keeping input decoupled from board logic.

**Placement flow:** drag → `DraggableBlock.OnEndDrag` → `DropHandler` →
`WoodokuGameManager.HandleDropRequest` → `BoardUI` screen-to-board conversion →
`BoardData.TryPlaceBlock` → `CellUpdate` events → `BoardUI` redraw.

## Notable in-progress state

- `WoodokuGameManager.Start` contains temporary test `SetCell` calls and a hardcoded random seed
  — scaffolding, not final behavior (see ROADMAP §5).
- Game-over is detected (`IsGameOver`) but only logs; no `GameState` enum, game-over UI, or
  restart yet (ROADMAP §7.3). The AI observation/action API (ROADMAP §8) is mostly unbuilt.
- Known typo kept consistent across call sites: `BoradData_OnCellUpdate` (ROADMAP flags fixing it).
