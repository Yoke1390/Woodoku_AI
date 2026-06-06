namespace Script.Core.Primitive
{
    public readonly struct GameStateData
    {
        public BoardSnapShot Board { get; }
        public Hand Hand { get; }
        public int Streak { get; }
        public int Score { get; }
        public Rng Rng { get; }
        public GameStatus Status { get; }

        public GameStateData(BoardSnapShot board, Hand hand, int streak, int score, Rng rng, GameStatus status)
        {
            Board = board;
            Hand = hand;
            Streak = streak;
            Score = score;
            Rng = rng;
            Status = status;
        }
    }
}
