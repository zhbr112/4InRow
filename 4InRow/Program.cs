using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

var board = new Board(7, 6);
while (true)
{
    Console.WriteLine("Введите ваш вариант:");
    int w;
    while (!int.TryParse(Console.ReadLine(), out w) || (w < 1 || w > board.Columns)) ;
    board.DropCoin(1, w - 1);

    // Stopwatch stopWatch = new Stopwatch();
    // stopWatch.Start();

    // var q = board.BestMove(1, 10);

    // stopWatch.Stop();
    // TimeSpan ts = stopWatch.Elapsed;
    // string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    //     ts.Hours, ts.Minutes, ts.Seconds,
    //     ts.Milliseconds / 10);
    // Console.WriteLine("RunTime " + elapsedTime);

    // Console.WriteLine(q + 1);
    // board.DropCoin(1, q);

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
    Console.WriteLine(board);


    // Console.WriteLine("Введите ваш вариант:");
    // int w;
    // while (!int.TryParse(Console.ReadLine(), out w) || (w < 1 || w > board.Columns));
    // board.DropCoin(2, w - 1);

    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();

    var q = board.BestMove(2, 9);

    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
       ts.Hours, ts.Minutes, ts.Seconds,
       ts.Milliseconds / 10);
    Console.WriteLine("RunTime " + elapsedTime);

    Console.WriteLine(q + 1);
    board.DropCoin(2, q);

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
}

Console.WriteLine("DONE");
Console.WriteLine(board);

public class Board
{
    public readonly int?[,] board;

    public int? _winner;

    public bool _changed;

    public Board(int cols, int rows)
    {
        //int cols = 7;
        //int rows = 6;
        Columns = cols;
        Rows = rows;
        board = new int?[cols, rows];
    }

    public Board(int cols, int rows, int?[,] _board)
    {
        //int cols = 7;
        //int rows = 6;
        Columns = cols;
        Rows = rows;
        board = _board;
    }

    public int Columns { get; }
    public int Rows { get; }

    public Board Copys()
    {
        return new Board(Columns, Rows, (int?[,])board.Clone());
    }

    public bool ColumnFree(int column)
    {
        return !board[column, 0].HasValue;
    }

    public bool DropCoin(int playerId, int column)
    {
        int row = 0;
        while (row < Rows && !board[column, row].HasValue)
        {
            row++;
        }

        if (row == 0)
            return false;
        board[column, row - 1] = playerId;
        _changed = true;
        return true;
    }

    public bool RemoveTopCoin(int column)
    {
        int row = 0;
        while (row < Rows && !board[column, row].HasValue)
        {
            row++;
        }

        if (row == Rows)
            return false;
        board[column, row] = null;
        _changed = true;
        return true;
    }
    public int EvaluateHorizontalSequences(int playerId)
    {
        int score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols - 3; col++)
            {
                // Check for potential winning sequences
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    if (board[row, col + k] == (playerId == 1 ? 1 : 2)) // AI piece
                        aiCount++;
                    else if (board[row, col + k] == (playerId == 1 ? 2 : 1)) // Human piece
                        humanCount++;
                }

                // Scoring logic
                score += ScoreSequence(aiCount, humanCount);
            }
        }

        return score;
    }
    private int EvaluateVerticalSequences(int playerId)
    {
        int score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // Iterate through each column
        for (int col = 0; col < cols; col++)
        {
            // Check vertical sequences of 4 cells
            for (int row = 0; row < rows - 3; row++)
            {
                int aiCount = 0;
                int humanCount = 0;

                // Check 4 consecutive vertical cells
                for (int k = 0; k < 4; k++)
                {
                    if (board[row + k, col] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row + k, col] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                // Score the vertical sequence
                score += ScoreSequence(aiCount, humanCount);
            }
        }

        return score;
    }

    private int EvaluateDiagonalSequences(int playerId)
    {
        int score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // Iterate through possible starting positions for diagonals
        for (int row = 0; row < rows - 3; row++)
        {
            for (int col = 0; col < cols - 3; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                // Check 4 consecutive diagonal cells (top-left to bottom-right)
                for (int k = 0; k < 4; k++)
                {
                    if (board[row + k, col + k] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row + k, col + k] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                // Score the diagonal sequence
                score += ScoreSequence(aiCount, humanCount);
            }
        }

        return score;
    }

    private int EvaluateReverseDiagonalSequences(int playerId)
    {
        int score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // Iterate through possible starting positions for reverse diagonals
        for (int row = 3; row < rows; row++)
        {
            // Start from the rightmost columns to scan right-to-left diagonals
            for (int col = 0; col < cols - 3; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                // Check 4 consecutive diagonal cells (top-right to bottom-left)
                for (int k = 0; k < 4; k++)
                {
                    if (board[row - k, col + k] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row - k, col + k] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                // Score the reverse diagonal sequence
                score += ScoreSequence(aiCount, humanCount);
            }
        }

        return score;
    }

    private int ScoreSequence(int aiCount, int humanCount)
    {
        // No pieces from either player
        if ((aiCount == 0 && humanCount == 0) || (aiCount > 0 && humanCount > 0))
            return 0;

        // Scoring based on sequence potential
        if (aiCount > 0)
        {
            return aiCount switch
            {
                1 => 2,
                2 => 20,
                3 => 200,
                _ => 0
            };
        }

        if (humanCount > 0)
        {
            return -1 * (humanCount switch
            {
                1 => 2,
                2 => 20,
                3 => 200,
                _ => 0
            });
        }

        return 0;
    }

    private int EvaluateBoard(int playerId, bool maximizingPlayer)
    {
        int score = 0;

        // Evaluate horizontal sequences
        score += EvaluateHorizontalSequences(playerId);

        // Evaluate vertical sequences
        score += EvaluateVerticalSequences(playerId);

        // Evaluate diagonal sequences (top-left to bottom-right)
        score += EvaluateDiagonalSequences(playerId);

        // Evaluate diagonal sequences (top-right to bottom-left)
        score += EvaluateReverseDiagonalSequences(playerId);

        // Adjust score based on the maximizing player
        return score;
    }


    public int BestMove(int playerId, int depth)
    {
        //var moves = new List<Tuple<int, int>>();

        //for (int i = 0; i < Columns; i++)
        //{
        //    Board _board = Copys();

        //    if (!_board.DropCoin(playerId, i))
        //        continue;
        //    moves.Add(Tuple.Create(i, _board.MinMax(depth, playerId)));
        //    _board.RemoveTopCoin(i);
        //}
        //Console.WriteLine($"{playerId} player: {String.Join(", ", moves.Select(x => $"{x.Item1 + 1} {x.Item2}"))}");

        var moves = Enumerable.Range(0, Columns).AsParallel().Select(i =>
        {
            Board _board = Copys();

            if (_board.DropCoin(playerId, i))
                return Tuple.Create(i, _board.MinMax(depth, playerId));
            return Tuple.Create(-1, -1);
        }
        ).Where(x => x.Item1 != -1).ToList();

        Console.WriteLine($"{playerId} player: {String.Join(", ", moves.Select(x => $"{x.Item1+1} {x.Item2}"))}");

        int maxMoveScore = moves.Max(t => t.Item2);
        return moves.Where(t => t.Item2 == maxMoveScore).OrderBy(t => Math.Abs(t.Item1 - Columns / 2)).First().Item1;
    }

    public int MinMax(int depth, int playerId, bool maximizingPlayer = false, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        //Console.WriteLine(this);
        var winner = Winner;
        if (winner == playerId)
            return (depth+1)*100000;
        if (winner == (playerId==1?2:1))
            return -((depth+1) * 100000);
        if (IsFull)
            return 0;

        if (depth == 0)
            return EvaluateBoard(playerId, maximizingPlayer);

        int bestValue = maximizingPlayer?int.MinValue:int.MaxValue;

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
                    if (!board[i, j].HasValue)
                        continue;

                    bool horizontal = i + 3 < Columns;
                    bool vertical = j + 3 < Rows;

                    if (!horizontal && !vertical)
                        continue;

                    bool forwardDiagonal = horizontal && vertical;
                    bool backwardDiagonal = vertical && i - 3 >= 0;

                    for (int k = 1; k < 4; k++)
                    {
                        horizontal = horizontal && board[i, j] == board[i + k, j];
                        vertical = vertical && board[i, j] == board[i, j + k];
                        forwardDiagonal = forwardDiagonal && board[i, j] == board[i + k, j + k];
                        backwardDiagonal = backwardDiagonal && board[i, j] == board[i - k, j + k];
                        if (!horizontal && !vertical && !forwardDiagonal && !backwardDiagonal)
                            break;
                    }

                    if (horizontal || vertical || forwardDiagonal || backwardDiagonal)
                    {
                        _winner = board[i, j];
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
                if (!board[i, 0].HasValue)
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
                builder.Append(board[i, j].HasValue ? board[i, j].Value.ToString() : " ").Append('|');
            }
            builder.AppendLine();
        }
        builder.AppendLine();
        builder.Append('|');
        for (int i = 0; i < Columns; i++)
        {
            builder.Append((i+1).ToString()).Append('|');
        }
        builder.AppendLine();
        return builder.ToString();
    }
}
