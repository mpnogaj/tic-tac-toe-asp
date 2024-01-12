using System.Text;

namespace TicTacToe.Models;

public class TicTacToe
{
    private const int N = 3;
    
    private int _moves = 0;
    
    public bool GameStarted { get; private set; }
    public bool GameFinished { get; private set; }
    
    public Player? Winner { get; private set; } = null;
    public Player CurrentTurn { get; private set; } = null!;
    private Player _nextTurn = null!;
    private readonly Dictionary<Player, Tile> _shapesDict = new();
    
    private readonly Tile[,] _board = new Tile[N, N];   

    public void StartGame(Player player1, Player player2)
    {
        for (var i = 0; i < N; i++)
        {
            for (var j = 0; j < N; j++)
            {
                _board[i, j] = Tile.Empty;
            }
        }

        _moves = 0;
        
        var randInt = Random.Shared.Next();
        _shapesDict[player1] = randInt % 2 == 0 ? Tile.Circle : Tile.Cross;
        _shapesDict[player2] = randInt % 2 == 0 ? Tile.Cross : Tile.Circle;

        CurrentTurn = _shapesDict[player1] == Tile.Cross ? player1 : player2;
        _nextTurn = CurrentTurn == player1 ? player2 : player1;

        GameStarted = true;
        GameFinished = false;
        Winner = null;
    }
    
    public bool CanMakeMove(int i, int j, Player player)
    {
        if (!_shapesDict.ContainsKey(player)) return false;
        if (CurrentTurn != player) return false;
        if (i < 0 || j < 0 || i >= N || j >= N) return false;
        if (GameFinished) return false;
        return _board[i, j] == Tile.Empty;
    }

    public void MakeMove(int i, int j, Player player)
    {
        if (!CanMakeMove(i, j, player)) return;
        _board[i, j] = _shapesDict[player];
        _moves++;
        CheckWinner(i, j, player);

        (CurrentTurn, _nextTurn) = (_nextTurn, CurrentTurn);
    }

    public void Surrender(Player player)
    {
        GameFinished = true;
        Winner = player == CurrentTurn ? _nextTurn : CurrentTurn;
    }

    private void CheckWinner(int i, int j, Player player)
    {
        var move = _board[i, j];
        
        //check row
        for (int x = 0; x < N; x++)
        {
            if (_board[i, x] != move)
                break;
            if (x == N - 1)
            {
                GameFinished = true;
                Winner = player;
                return;
            }
        }
        
        //check column
        for (int x = 0; x < N; x++)
        {
            if (_board[x, j] != move)
                break;
            if (x == N - 1)
            {
                GameFinished = true;
                Winner = player;
                return;
            }
        }

        //check dag
        if (i == j)
        {
            for (int x = 0; x < N; x++)
            {
                if (_board[x, x] != move)
                    break;
                if (x == N - 1)
                {
                    GameFinished = true;
                    Winner = player;
                    return;
                }
            }
        }

        //check dag2
        if (i + j == N - 1)
        {
            for (int x = 0; x < N; x++)
            {
                if (_board[x, N - 1 - x] != move)
                    break;
                if (x == N - 1)
                {
                    GameFinished = true;
                    Winner = player;
                    return;
                }
            }
        }
        
        //draw
        if (_moves == N * N)
        {
            GameFinished = true;
            Winner = null;
            return;
        }

        GameFinished = false;
        Winner = null;
    }

    public string BoardToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < N; i++)
        {
            for (var j = 0; j < N; j++)
            {
                var chr = _board[i, j] switch
                {
                    Tile.Empty => '0',
                    Tile.Circle => '1',
                    Tile.Cross => '2',
                    _ => throw new ArgumentOutOfRangeException()
                };
                sb.Append(chr);
            }
        }

        return sb.ToString();
    }
}

public enum Tile
{
    Empty,
    Circle,
    Cross
}