using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AgentRunner : MonoBehaviour // WoodokuGameManager の人間入力を agent コルーチンに差し替えた版
{
    [SerializeField]
    private float stepDelay = 0.3f;

    [SerializeField]
    private GameSetting gameSetting;

    [SerializeField]
    private BoardUI boardUI;

    [SerializeField]
    private HandUI handUI;

    [SerializeField]
    private GameOverUI gameOverUI;

    [SerializeField]
    private ScoreUI scoreUI;

    private GameSession session;
    private IWoodokuAgent agent;

    private List<BlockShape> shapes;

    void Start()
    {
        shapes = Resources.LoadAll<BlockData>("").Select(b => b.ToShape()).ToList();

        session = new(gameSetting.GridSize, shapes, 3, seed: 0);
        boardUI.Initialize(session.Board);
        handUI.Initialize(NoOpDrop, boardUI.CellSize, session.Hands); // 人間入力は殺す

        session.Score.ScoreUpdate += scoreUI.UpdateScore;
        session.GameOver += () => gameOverUI.Show();
        gameOverUI.Restart += OnRestart;
        StartCoroutine(RunNewSession());
    }

    private void OnRestart()
    {
        StartCoroutine(RunNewSession());
    }

    private IEnumerator RunNewSession()
    {
        agent = new RandomAgent();

        session.Begin(seed: 0);
        yield return StartCoroutine(Run());
    }

    private static bool NoOpDrop(PointerEventData e, int slot) => false;

    private IEnumerator Run()
    {
        while (session.State == GameState.Playing)
        {
            var legal = session.GetLegalActions().ToList();
            if (legal.Count == 0)
                break;
            Observation observation = new(session.Board, session.Hands);
            var a = agent.SelectAction(observation, legal);
            session.TryPlaceBlock(a); // UI は3イベントで自動追従
            yield return new WaitForSeconds(stepDelay);
        }
    }
}
