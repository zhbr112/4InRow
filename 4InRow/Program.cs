using System.Numerics;
using System.Text;

var board = new Board(7, 6);
while (true)
{
    var q=board.BestMove(1, 7);
    Console.WriteLine(q+1);
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
    var w = board.BestMove(2, 7);
    Console.WriteLine(w + 1);
    board.DropCoin(2, w);
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
    private readonly int?[,] board;

    private int? _winner;

    private bool _changed;

    private Random random;

    public Board(int cols, int rows)
    {
        Columns = cols;
        Rows = rows;
        board = new int?[cols, rows];
        random = new Random();
    }

    public int Columns { get; }
    public int Rows { get; }

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

    private int EvaluateHorizontalSequences(int playerId)
    {
        int score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col <= cols - 4; col++)
            {
                // Check for potential winning sequences
                int aiCount = 0;
                int humanCount = 0;

                for (int k = 0; k < 4; k++)
                {
                    if (board[row, col + k] == (playerId==1?1:2)) // AI piece
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
            for (int row = 0; row <= rows - 4; row++)
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
        for (int row = 0; row <= rows - 4; row++)
        {
            for (int col = 0; col <= cols - 4; col++)
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
        for (int row = 0; row <= rows - 4; row++)
        {
            // Start from the rightmost columns to scan right-to-left diagonals
            for (int col = cols - 1; col >= 3; col--)
            {
                int aiCount = 0;
                int humanCount = 0;

                // Check 4 consecutive diagonal cells (top-right to bottom-left)
                for (int k = 0; k < 4; k++)
                {
                    if (board[row + k, col - k] == (playerId == 1 ? 1 : 2))
                        aiCount++;
                    else if (board[row + k, col - k] == (playerId == 1 ? 2 : 1))
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
        if (aiCount == 0 && humanCount == 0)
            return 0;

        // Potential threats and opportunities
        if (aiCount > 0 && humanCount > 0)
            return 0; // Blocked sequence

        // AI four-in-a-row
        if (aiCount == 4)
            return 1000;

        // Human four-in-a-row
        if (humanCount == 4)
            return -1000;

        // Scoring based on sequence potential
        if (aiCount > 0)
        {
            return aiCount switch
            {
                1 => 1,
                2 => 10,
                3 => 100,
                _ => 0
            };
        }

        if (humanCount > 0)
        {
            return -1 * (humanCount switch
            {
                1 => 1 ,
                2 => 10,
                3 => 100,
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
        var moves = new List<Tuple<int, int>>();
        for (int i = 0; i < Columns; i++)
        {
            if (!DropCoin(playerId, i))
                continue;
            moves.Add(Tuple.Create(i, MinMax(depth, playerId)));
            RemoveTopCoin(i);
        }
        Console.WriteLine($"{playerId} player: {String.Join(", ", moves.Select(x => $"{x.Item1+1} {x.Item2}"))}");

        int maxMoveScore = moves.Max(t => t.Item2);
        var bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
        return bestMoves[random.Next(0, bestMoves.Count)].Item1;
    }

    int MinMax(int depth, int playerId, bool maximizingPlayer = false, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        if (depth <= 0)
            return EvaluateBoard(playerId, maximizingPlayer);

        var winner = Winner;
        if (winner == playerId)
            return depth*100000;
        if (winner == (playerId==1?2:1))
            return -(depth * 100000);
        if (IsFull)
            return 0;

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
