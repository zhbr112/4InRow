using System;
using System.Runtime.InteropServices;
using System.Text;


var board = new Board(7, 6);
var random = new Random();


while (true)
{
    board.DropCoin(1, board.BestMove(1, 10));
    
    

    if (board.Winner == 1)
    {
        Console.WriteLine("You win!");
        break;
    }

    if (board.IsFull)
    {
        Console.WriteLine("Tie!");
        break;
    }

    board.DropCoin(2, board.BestMove(2, 6));
    if (board.Winner == 2)
    {
        Console.WriteLine("You lost!");
        break;
    }

    if (board.IsFull)
    {
        Console.WriteLine("Tie!");
        break;
    }
    Console.WriteLine(board);
    {
        //Console.WriteLine("Pick a column 1 -8");
        //int move;

        //var moves = new List<Tuple<int, int>>();
        //for (int i = 0; i < board.Columns; i++)
        //{
        //    if (!board.DropCoin(1, i))
        //        continue;
        //    moves.Add(Tuple.Create(i, MinMax1(9, board, false)));
        //    board.RemoveTopCoin(i);
        //}
        //int maxMoveScore = moves.Max(t => t.Item2);
        //var bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
        //move = bestMoves[random.Next(0, bestMoves.Count)].Item1;

        //if (!board.DropCoin(1, move))
        //{
        //    Console.WriteLine("That column is full, pick another one");
        //    continue;
        //}
        //if (board.Winner == 1)
        //{
        //    Console.WriteLine(board);
        //    Console.WriteLine("You loss!");
        //    break;
        //}

        //Console.WriteLine(board);
        //if (!int.TryParse(Console.ReadLine(), out move) || move < 1 || move > 8)
        //{
        //    Console.WriteLine("Must enter a number 1-8.");
        //    continue;
        //}

        //if (!board.DropCoin(2, move - 1))
        //{
        //    Console.WriteLine("That column is full, pick another one");
        //    continue;
        //}

        //if (board.Winner == 2)
        //{
        //    Console.WriteLine(board);
        //    Console.WriteLine("You win!");
        //    break;
        //}

        //if (board.IsFull)
        //{
        //    Console.WriteLine(board);
        //    Console.WriteLine("Tie!");
        //    break;
        //}

        //var moves = new List<Tuple<int, int>>();
        //for (int i = 0; i < board.Columns; i++)
        //{
        //    if (!board.DropCoin(2, i))
        //        continue;
        //    moves.Add(Tuple.Create(i, MinMax(9, board, false)));
        //    board.RemoveTopCoin(i);
        //}

        //var maxMoveScore = moves.Max(t => t.Item2);
        //var bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
        //board.DropCoin(2, bestMoves[random.Next(0, bestMoves.Count)].Item1);
        //Console.WriteLine(board);

        //if (board.Winner == 2)
        //{
        //    Console.WriteLine("You lost!");
        //    break;
        //}

        //if (board.IsFull)
        //{
        //    Console.WriteLine("Tie!");
        //    break;
        //}
    }
}
Console.WriteLine(board);
Console.WriteLine("DONE");



static int MinMax(int depth, Board board, bool maximizingPlayer, int alpha = int.MinValue, int beta = int.MaxValue)
{
    if (depth <= 0)
        return 0;

    var winner = board.Winner;
    if (winner == 2)
        return depth;
    if (winner == 1)
        return -depth;
    if (board.IsFull)
        return 0;


    int bestValue = maximizingPlayer ? -1 : 1;
    for (int i = 0; i < board.Columns; i++)
    {
        if (!board.DropCoin(maximizingPlayer ? 2 : 1, i))
            continue;
        int v = MinMax(depth - 1, board, !maximizingPlayer, alpha, beta);
        bestValue = maximizingPlayer ? Math.Max(bestValue, v) : Math.Min(bestValue, v);
        board.RemoveTopCoin(i);

        if (maximizingPlayer)
            alpha = Math.Max(alpha, bestValue);
        else
            beta = Math.Min(beta, bestValue);
        if (beta <= alpha)
            break;
    }

    return bestValue;
}

static int MinMax1(int depth, Board board, bool maximizingPlayer)
{
    if (depth <= 0)
        return 0;

    var winner = board.Winner;
    if (winner == 2)
        return -depth;
    if (winner == 1)
        return depth;
    if (board.IsFull)
        return 0;


    int bestValue = maximizingPlayer ? -1 : 1;
    for (int i = 0; i < board.Columns; i++)
    {
        if (!board.DropCoin(maximizingPlayer ? 1 : 2, i))
            continue;
        int v = MinMax1(depth - 1, board, !maximizingPlayer);
        bestValue = maximizingPlayer ? Math.Max(bestValue, v) : Math.Min(bestValue, v);
        board.RemoveTopCoin(i);    
    }

    return bestValue;
}

public class Board
{
    private readonly int?[,] _board;

    private int? _winner;

    private bool _changed;

    private Random random;

    public Board(int cols, int rows)
    {
        Columns = cols;
        Rows = rows;
        _board = new int?[cols, rows];
        random = new Random();
    }

    public int Columns { get; }
    public int Rows { get; }

    public bool ColumnFree(int column)
    {
        return !_board[column, 0].HasValue;
    }

    public bool DropCoin(int playerId, int column)
    {
        int row = 0;
        while (row < Rows && !_board[column, row].HasValue)
        {
            row++;
        }

        if (row == 0)
            return false;
        _board[column, row - 1] = playerId;
        _changed = true;
        return true;
    }

    public bool RemoveTopCoin(int column)
    {
        int row = 0;
        while (row < Rows && !_board[column, row].HasValue)
        {
            row++;
        }

        if (row == Rows)
            return false;
        _board[column, row] = null;
        _changed = true;
        return true;
    }

    public int BestMove(int playerId, int depth)
    {
        var moves = new List<Tuple<int, int>>();
        for (int i = 0; i < Columns; i++)
        {
            if (!DropCoin(playerId, i))
                continue;
            moves.Add(Tuple.Create(i, MinMax(depth, playerId, false)));
            RemoveTopCoin(i);
        }
        Console.WriteLine($"{playerId} Player: {String.Join(", ", moves.Select(x => $"{x.Item1+1} {x.Item2}"))}");
        int maxMoveScore = moves.Max(t => t.Item2);
        var bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
        return bestMoves[random.Next(0, bestMoves.Count)].Item1;
    }

    int MinMax(int depth, int playerId, bool maximizingPlayer, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        if (depth <= 0)
            return 0;

        var winner = Winner;
        if (winner == playerId)
            return depth+2;
        if (winner == (playerId==1?2:1))
            return -depth-2;
        if (IsFull)
            return 0;
        int bestValue = maximizingPlayer ? int.MinValue : int.MaxValue;

        for (int i = 0; i < Columns; i++)
        {
            if (!DropCoin(maximizingPlayer ? playerId : (playerId == 1 ? 2 : 1), i))
                continue;
            int v = MinMax(depth - 1, playerId, !maximizingPlayer, alpha, beta);
            bestValue = maximizingPlayer ? Math.Max(bestValue, v) : Math.Min(bestValue, v);
            RemoveTopCoin(i);

            if (maximizingPlayer)
                alpha = Math.Max(alpha, bestValue);
            else
                beta = Math.Min(beta, bestValue);

            if (beta <= alpha)
                i++;
        }

        return bestValue;
    }

    public int? Winner
    {
        get
        {
            if (!_changed)
                return _winner;

            _changed = false;
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    if (!_board[i, j].HasValue)
                        continue;

                    bool horizontal = i + 3 < Columns;
                    bool vertical = j + 3 < Rows;

                    if (!horizontal && !vertical)
                        continue;

                    bool forwardDiagonal = horizontal && vertical;
                    bool backwardDiagonal = vertical && i - 3 >= 0;

                    for (int k = 1; k < 4; k++)
                    {
                        horizontal = horizontal && _board[i, j] == _board[i + k, j];
                        vertical = vertical && _board[i, j] == _board[i, j + k];
                        forwardDiagonal = forwardDiagonal && _board[i, j] == _board[i + k, j + k];
                        backwardDiagonal = backwardDiagonal && _board[i, j] == _board[i - k, j + k];
                        if (!horizontal && !vertical && !forwardDiagonal && !backwardDiagonal)
                            break;
                    }

                    if (horizontal || vertical || forwardDiagonal || backwardDiagonal)
                    {
                        _winner = _board[i, j];
                        return _winner;
                    }
                }
            }

            _winner = null;
            return _winner;
        }
    }

    public bool IsFull
    {
        get
        {
            for (int i = 0; i < Columns; i++)
            {
                if (!_board[i, 0].HasValue)
                    return false;
            }

            return true;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (int j = 0; j < Rows; j++)
        {
            builder.Append('|');
            for (int i = 0; i < Columns; i++)
            {
                builder.Append(_board[i, j].HasValue ? _board[i, j].Value.ToString() : " ").Append('|');
            }
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
