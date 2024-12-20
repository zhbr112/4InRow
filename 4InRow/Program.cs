using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;



var board = new Board(7, 6);

while (true)
{
    //Console.WriteLine("Введите ваш вариант:");
    //int w;
    //while (!int.TryParse(Console.ReadLine(), out w) || (w < 1 || w > board.Columns)) ;
    //board.DropCoin(1, w - 1);

    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();

    var q = board.BestMove(1, 9);

    stopWatch.Stop();
    TimeSpan ts = stopWatch.Elapsed;
    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
    Console.WriteLine("RunTime " + elapsedTime);

    Console.WriteLine(q + 1);
    board.DropCoin(1, q);

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


    Console.WriteLine("Введите ваш вариант:");
    int w;
    while (!int.TryParse(Console.ReadLine(), out w) || (w < 1 || w > board.Columns)) ;
    board.DropCoin(2, w - 1);

    //Stopwatch stopWatch = new Stopwatch();
    //stopWatch.Start();

    //var q = board.BestMove(2, 9);

    //stopWatch.Stop();
    //TimeSpan ts = stopWatch.Elapsed;
    //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    //   ts.Hours, ts.Minutes, ts.Seconds,
    //   ts.Milliseconds / 10);
    //Console.WriteLine("RunTime " + elapsedTime);

    //Console.WriteLine(q + 1);
    //board.DropCoin(2, q);

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
    public readonly int? [][] board;

    public int? _winner;

    public bool _changed;

    public Board(int cols, int rows)
    {
        //int cols = 7;
        //int rows = 6;
        Columns = cols;
        Rows = rows;
        board = new int?[Rows][];
        for(var i = 0; i < Rows; i++)
        {
            board[i] = new int?[Columns];
        }
    }

    public Board(int cols, int rows, int?[][] _board)
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
        var board_=new int?[Rows][];
        for(int i = 0;i<Rows;i++)
        {
            board_[i] = (int?[])board[i].Clone();
        }
        return new Board(Columns, Rows, board_);
    }

    public bool ColumnFree(int column)
    {
        return !board[0][column].HasValue;
    }

    public bool DropCoin(int playerId, int column)
    {
        int row = 0;
        while (row < Rows && !board[row][column].HasValue)
        {
            row++;
        }

        if (row == 0)
            return false;
        board[row - 1][column] = playerId;
        _changed = true;
        return true;
    }

    public bool RemoveTopCoin(int column)
    {
        int row = 0;
        while (row < Rows && !board[row][column].HasValue)
        {
            row++;
        }

        if (row == Rows)
            return false;
        board[row][column] = null;
        _changed = true;
        return true;
    }
    //public int EvaluateHorizontalSequences(int playerId)
    //{
    //    int score = 0;
    //    int rows = board.GetLength(0);
    //    int cols = board.GetLength(1);

    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int col = 0; col < cols - 3; col++)
    //        {
    //            // Check for potential winning sequences
    //            int aiCount = 0;
    //            int humanCount = 0;

    //            for (int k = 0; k < 4; k++)
    //            {
    //                if (board[row, col + k] == (playerId == 1 ? 1 : 2)) // AI piece
    //                    aiCount++;
    //                else if (board[row, col + k] == (playerId == 1 ? 2 : 1)) // Human piece
    //                    humanCount++;
    //            }

    //            // Scoring logic
    //            score += ScoreSequence(aiCount, humanCount);
    //        }
    //    }

    //    return score;
    //}
    //private int EvaluateVerticalSequences(int playerId)
    //{
    //    int score = 0;
    //    int rows = board.GetLength(0);
    //    int cols = board.GetLength(1);

    //    // Iterate through each column
    //    for (int col = 0; col < cols; col++)
    //    {
    //        // Check vertical sequences of 4 cells
    //        for (int row = 0; row < rows - 3; row++)
    //        {
    //            int aiCount = 0;
    //            int humanCount = 0;

    //            // Check 4 consecutive vertical cells
    //            for (int k = 0; k < 4; k++)
    //            {
    //                if (board[row + k, col] == (playerId == 1 ? 1 : 2))
    //                    aiCount++;
    //                else if (board[row + k, col] == (playerId == 1 ? 2 : 1))
    //                    humanCount++;
    //            }

    //            // Score the vertical sequence
    //            score += ScoreSequence(aiCount, humanCount);
    //        }
    //    }

    //    return score;
    //}

    //private int EvaluateDiagonalSequences(int playerId)
    //{
    //    int score = 0;
    //    int rows = board.GetLength(0);
    //    int cols = board.GetLength(1);

    //    // Iterate through possible starting positions for diagonals
    //    for (int row = 0; row < rows - 3; row++)
    //    {
    //        for (int col = 0; col < cols - 3; col++)
    //        {
    //            int aiCount = 0;
    //            int humanCount = 0;

    //            // Check 4 consecutive diagonal cells (top-left to bottom-right)
    //            for (int k = 0; k < 4; k++)
    //            {
    //                if (board[row + k, col + k] == (playerId == 1 ? 1 : 2))
    //                    aiCount++;
    //                else if (board[row + k, col + k] == (playerId == 1 ? 2 : 1))
    //                    humanCount++;
    //            }

    //            // Score the diagonal sequence
    //            score += ScoreSequence(aiCount, humanCount);
    //        }
    //    }

    //    return score;
    //}

    //private int EvaluateReverseDiagonalSequences(int playerId)
    //{
    //    int score = 0;
    //    int rows = board.GetLength(0);
    //    int cols = board.GetLength(1);

    //    // Iterate through possible starting positions for reverse diagonals
    //    for (int row = 3; row < rows; row++)
    //    {
    //        // Start from the rightmost columns to scan right-to-left diagonals
    //        for (int col = 0; col < cols - 3; col++)
    //        {
    //            int aiCount = 0;
    //            int humanCount = 0;

    //            // Check 4 consecutive diagonal cells (top-right to bottom-left)
    //            for (int k = 0; k < 4; k++)
    //            {
    //                if (board[row - k, col + k] == (playerId == 1 ? 1 : 2))
    //                    aiCount++;
    //                else if (board[row - k, col + k] == (playerId == 1 ? 2 : 1))
    //                    humanCount++;
    //            }

    //            // Score the reverse diagonal sequence
    //            score += ScoreSequence(aiCount, humanCount);
    //        }
    //    }

    //    return score;
    //}

    private static int ScoreSequence(int aiCount, int humanCount)
    {
        // No pieces from either player
        if ((aiCount == 0 && humanCount == 0) || (aiCount > 0 && humanCount > 0))
            return 0;

        // Scoring based on sequence potential
        else if (aiCount > 0)
        {
            return aiCount switch
            {
                1 => 2,
                2 => 20,
                3 => 200,
                _ => 0
            };
        }

        else
        {
            return humanCount switch
            {
                1 => -2,
                2 => -20,
                3 => -200,
                _ => 0
            };
        }
    }
    private static int ScoreSequence2(int playerId, int? win, int count)
    {
        // Scoring based on sequence potential
        if (playerId == win)
        {
            return count switch
            {
                1 => 2,
                2 => 20,
                3 => 200,
                4 => 2000,
                _ => 0
            };
        }

        else
        {
            return count switch
            {
                1 => -2,
                2 => -20,
                3 => -200,
                4 => -2000,
                _ => 0
            };
        }
    }

    private int EvaluateBoard(int playerId, bool maximizingPlayer)
    {
        int score = 0;

        //for (int i = 0; i < Rows; i++)
        //{
        //    for (int j = 0; j < Columns; j++)
        //    {
        //        var win = board[i][j];
        //        var win1 = board[i][j];
        //        var win2 = board[i][j];
        //        var win3 = board[i][j];
        //        var win4 = board[i][j];
        //        if (win is null) win = 1;


        //        bool horizontal = i + 3 < Rows;
        //        bool vertical = j + 3 < Columns;

        //        if (!horizontal && !vertical)
        //            continue;

        //        bool forwardDiagonal = horizontal && vertical;
        //        bool backwardDiagonal = vertical && i - 3 >= 0;

        //        var q1 = 1;
        //        var q2 = 1;
        //        var q3 = 1;
        //        var q4 = 1;

        //        for (int k = 1; k < 4; k++)
        //        {
        //            if (win is null && board[i + k][j] is not null) win= board[i + k][j];

        //            horizontal = horizontal && win is not null && (board[i + k][j] is null || board[i + k][j] == win);
        //            vertical = vertical && (board[i][j + k] is null || board[i][j + k] == win);
        //            forwardDiagonal = forwardDiagonal && (board[i + k][j + k] is null || board[i + k][j + k] == win);
        //            backwardDiagonal = backwardDiagonal && (board[i - k][j + k] is null || board[i - k][j + k] == win);

        //            if (horizontal && (win == board[i + k][j]))
        //            {
        //                q1++;
        //            }

        //            if (vertical && (win == board[i][j + k]))
        //            {
        //                q2++;
        //            }

        //            if (forwardDiagonal && (win == board[i + k][j + k]))
        //            {
        //                q3++;
        //            }

        //            if (backwardDiagonal && (win == board[i - k][j + k]))
        //            {
        //                q4++;
        //            }
        //        }
        //        if (horizontal) score += ScoreSequence2(playerId, win, q1);
        //        if (vertical) score += ScoreSequence2(playerId, win, q2);
        //        if (forwardDiagonal) score += ScoreSequence2(playerId,  win, q3);
        //        if (backwardDiagonal) score += ScoreSequence2(playerId, win, q4);
        //    }
        //}

        //for (int row = 0; row < Rows; row++)
        //{
        //    for (int col = 0; col < Columns; col++)
        //    {
        //        bool horizontal = row + 3 < Rows;
        //        bool vertical = col + 3 < Columns;

        //        if (!horizontal && !vertical)
        //            continue;

        //        bool forwardDiagonal = horizontal && vertical;
        //        bool backwardDiagonal = vertical && row - 3 >= 0;
        //        if (horizontal)
        //        {
        //            int aiCount = 0;
        //            int humanCount = 0;

        //            for (int k = 0; k < 4; k++)
        //            {
        //                //aiCount += Convert.ToInt32(board[row][col + k] == (playerId == 1 ? 1 : 2));
        //                //humanCount += Convert.ToInt32(board[row][col + k] == (playerId == 1 ? 2 : 1));
        //                if (board[row + k][col] == (playerId == 1 ? 1 : 2)) // AI piece
        //                    aiCount++;
        //                else if (board[row + k][col] == (playerId == 1 ? 2 : 1)) // Human piece
        //                    humanCount++;
        //            }

        //            score += ScoreSequence(aiCount, humanCount);
        //        }
        //        if (vertical)
        //        {
        //            int aiCount = 0;
        //            int humanCount = 0;

        //            for (int k = 0; k < 4; k++)
        //            {
        //                //aiCount += Convert.ToInt32(board[row+k][col] == (playerId == 1 ? 1 : 2));
        //                //humanCount += Convert.ToInt32(board[row+k][col] == (playerId == 1 ? 2 : 1));
        //                if (board[row][col + k] == (playerId == 1 ? 1 : 2))
        //                    aiCount++;
        //                else if (board[row][col + k] == (playerId == 1 ? 2 : 1))
        //                    humanCount++;
        //            }

        //            score += ScoreSequence(aiCount, humanCount);
        //        }
        //        if (forwardDiagonal)
        //        {
        //            int aiCount = 0;
        //            int humanCount = 0;

        //            for (int k = 0; k < 4; k++)
        //            {
        //                //aiCount += Convert.ToInt32(board[row + k][col + k] == (playerId == 1 ? 1 : 2));
        //                //humanCount += Convert.ToInt32(board[row + k][col + k] == (playerId == 1 ? 2 : 1));
        //                if (board[row + k][col + k] == (playerId == 1 ? 1 : 2))
        //                    aiCount++;
        //                else if (board[row + k][col + k] == (playerId == 1 ? 2 : 1))
        //                    humanCount++;
        //            }

        //            score += ScoreSequence(aiCount, humanCount);
        //        }
        //        if (backwardDiagonal)
        //        {
        //            int aiCount = 0;
        //            int humanCount = 0;

        //            for (int k = 0; k < 4; k++)
        //            {
        //                //aiCount += Convert.ToInt32(board[row - k][col + k] == (playerId == 1 ? 1 : 2));
        //                //humanCount += Convert.ToInt32(board[row - k][col + k] == (playerId == 1 ? 2 : 1));
        //                if (board[row - k][col + k] == (playerId == 1 ? 1 : 2))
        //                    aiCount++;
        //                else if (board[row - k][col + k] == (playerId == 1 ? 2 : 1))
        //                    humanCount++;
        //            }

        //            score += ScoreSequence(aiCount, humanCount);
        //        }
        //    }
        //}

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    //aiCount += Convert.ToInt32(board[row][col + k] == (playerId == 1 ? 1 : 2));
                    //humanCount += Convert.ToInt32(board[row][col + k] == (playerId == 1 ? 2 : 1));
                    if (board[row][col + k] == (playerId == 1 ? 1 : 2)) // AI piece
                        aiCount++;
                    else if (board[row][col + k] == (playerId == 1 ? 2 : 1)) // Human piece
                        humanCount++;
                }

                score += ScoreSequence(aiCount, humanCount);
            }
        }
        for (int row = 0; row < Rows - 3; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    //aiCount += Convert.ToInt32(board[row+k][col] == (playerId == 1 ? 1 : 2));
                    //humanCount += Convert.ToInt32(board[row+k][col] == (playerId == 1 ? 2 : 1));
                    if (board[row + k][col] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row + k][col] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                score += ScoreSequence(aiCount, humanCount);
            }
        }

        for (int row = 0; row < Rows - 3; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    //aiCount += Convert.ToInt32(board[row + k][col + k] == (playerId == 1 ? 1 : 2));
                    //humanCount += Convert.ToInt32(board[row + k][col + k] == (playerId == 1 ? 2 : 1));
                    if (board[row + k][col + k] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row + k][col + k] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                score += ScoreSequence(aiCount, humanCount);
            }
        }

        for (int row = 3; row < Rows; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    //aiCount += Convert.ToInt32(board[row - k][col + k] == (playerId == 1 ? 1 : 2));
                    //humanCount += Convert.ToInt32(board[row - k][col + k] == (playerId == 1 ? 2 : 1));
                    if (board[row - k][col + k] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row - k][col + k] == (playerId == 1 ? 2 : 1))
                        humanCount++;
                }

                score += ScoreSequence(aiCount, humanCount);
            }
        }

        return score;
    }


    public int BestMove(int playerId, int depth)
    {
        //var moves = new List<Tuple<int, int>>();
        //for(var i=0; i<Columns; i++)
        //{
        //    Board _board = Copys();

        //    if (!DropCoin(playerId, i))
        //        break;
        //    moves.Add(Tuple.Create(i, MinMax(depth, playerId)));
        //}

        var moves = new ConcurrentBag<Tuple<int, int>>();
        Parallel.For(0, Columns, i =>
        {
            Board _board = Copys();

            if (!_board.DropCoin(playerId, i))
                return;
            moves.Add(Tuple.Create(i, _board.MinMax(depth, playerId)));
        });

        //var moves = Enumerable.Range(0, Columns).AsParallel().Select(i =>
        //{
        //    Board _board = Copys();

        //    if (_board.DropCoin(playerId, i))
        //        return Tuple.Create(i, _board.MinMax(depth, playerId));
        //    return Tuple.Create(-1, -1);
        //}
        //).Where(x => x.Item1 != -1).ToList();

        Console.WriteLine($"{playerId} player: {String.Join(", ", moves.OrderBy(x => x.Item1).Select(x => $"{x.Item1+1} {x.Item2}"))}");

        return moves.OrderByDescending(t => t.Item2).ThenBy(t => Math.Abs(t.Item1 - Columns / 2)).First().Item1;
    }

    public int MinMax(int depth, int playerId, bool maximizingPlayer = false, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        //Console.WriteLine(this);
        var winner = Winner;
        if (winner == playerId)
            return (depth + 1) * 100000;
        if (winner == (playerId==1?2:1))
            return -(depth+1) * 100000;
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
                break;
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
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!board[i][j].HasValue)
                        continue;

                    bool horizontal = i + 3 < Rows;
                    bool vertical = j + 3 < Columns;

                    if (!horizontal && !vertical)
                        continue;

                    bool forwardDiagonal = horizontal && vertical;
                    bool backwardDiagonal = vertical && i - 3 >= 0;

                    for (int k = 1; k < 4; k++)
                    {
                        horizontal = horizontal && board[i][j] == board[i + k][j];
                        vertical = vertical && board[i][j] == board[i][j + k];
                        forwardDiagonal = forwardDiagonal && board[i][j] == board[i + k][j + k];
                        backwardDiagonal = backwardDiagonal && board[i][j] == board[i - k][j + k];
                        if (!horizontal && !vertical && !forwardDiagonal && !backwardDiagonal)
                            break;
                    }

                    if (horizontal || vertical || forwardDiagonal || backwardDiagonal)
                    {
                        _winner = board[i][j];
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
                if (!board[0][i].HasValue)
                    return false;
            }

            return true;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (int i = 0; i < Rows; i++)
        {
            builder.Append('|');
            for (int j = 0; j < Columns; j++)
            {
                builder.Append(board[i][j].HasValue ? board[i][j].Value.ToString() : " ").Append('|');
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
