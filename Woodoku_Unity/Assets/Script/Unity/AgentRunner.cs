using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Core;
using Script.Core.Agents;
using Script.Core.Primitive;
using Script.Unity.Board;
using Script.Unity.Hand;
using UnityEngine;

namespace Script.Unity
{
    public class AgentRunner : MonoBehaviour // WoodokuGameManager の人間入力を agent コルーチンに差し替えた版
    {
        [SerializeField] private BoardUI boardUI;

        [SerializeField] private GameOverUI gameOverUI;

        [SerializeField] private GameSetting gameSetting;

        [SerializeField] private HandUI handUI;

        [SerializeField] private ScoreUI scoreUI;

        [SerializeField] private int seed;

        [SerializeField] private float stepDelay = 0.3f;
        private IWoodokuAgent _agent;

        private GameSession _session;

        private List<BlockShape> _shapes;

        private void Start()
        {
            _shapes = Resources.LoadAll<BlockData>("").Select(b => b.ToShape()).ToList();

            _session = new GameSession(gameSetting.GridSize, _shapes, 3, 0);
            boardUI.Initialize(_session.Board, _session.BoardEvent);
            handUI.Initialize(NoOpDrop, boardUI.CellSize, _session.Hands); // 人間入力は殺す

            _session.Score.ScoreUpdate += scoreUI.UpdateScore;
            _session.GameOver += () => gameOverUI.Show();
            gameOverUI.Restart += OnRestart;
            StartCoroutine(RunNewSession());
        }

        private void OnRestart()
        {
            StartCoroutine(RunNewSession());
        }

        private IEnumerator RunNewSession()
        {
            _agent = new RandomAgent();

            _session.Begin(seed);
            yield return StartCoroutine(Run());
        }

        private static bool NoOpDrop(Vector2 v, int slot)
        {
            return false;
        }

        private IEnumerator Run()
        {
            while (_session.Status == GameStatus.Playing)
            {
                List<AgentAction> legal = _session.GetLegalActions().ToList();
                if (legal.Count == 0)
                    break;
                Observation observation = new(_session.Board, _session.Hands);
                AgentAction a = _agent.SelectAction(observation, legal);
                _session.TryPlaceBlock(a); // UI は3イベントで自動追従
                yield return new WaitForSeconds(stepDelay);
            }
        }
    }
}