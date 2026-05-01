class BoardData
{
    private int[,] board;

    public BoardData()
    {
        board = new int[9, 9];
    }

    private void addBlock(int x, int y)
    {
        board[x, y] = 1;
    }
}
