using System.Text;
using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ZumaGame;

// LeetCode 488. Zuma Game: level-by-level BFS over "insert one hand ball, then
// cascade-collapse any run of 3+ same-colored balls" states, using this repo's own
// Queue<string> as the frontier and Set<string> to dedupe (board, remaining-hand)
// states already tried - the exact RemoveInvalidParentheses/LC301 shape, since each
// BFS level here corresponds to using one more ball from the hand, so the first
// level whose board empties out holds the minimum number of balls needed.
public sealed partial class ZumaGameTests
{
    [Theory]
    [InlineData("WRRBBW", "RB", -1)]
    [InlineData("WWRRBBWW", "WRBRW", 2)]
    [InlineData("G", "GGGGG", 2)]
    [InlineData("RBYYBBRRB", "YRBGB", 3)]
    public void FindMinStep_LeetCodeExamples_ReturnsMinimumBallsNeededOrNegativeOne(
        string board, string hand, int expected)
        => Assert.Equal(expected, FindMinStep(board, hand));

    private static int FindMinStep(string board, string hand)
    {
        var visited = new Set<string>();
        var queue = new RepoQueue();
        var start = board + "|" + SortChars(hand);
        visited.TryAdd(start);
        queue.Enqueue(start);
        var moves = 0;

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;

            for (var i = 0; i < levelSize; i++)
            {
                queue.TryDequeue(out var state);
                var separator = state.IndexOf('|');
                var currentBoard = state[..separator];
                var currentHand = state[(separator + 1)..];

                if (currentBoard.Length == 0)
                {
                    return moves;
                }

                for (var pos = 0; pos <= currentBoard.Length; pos++)
                {
                    for (var h = 0; h < currentHand.Length; h++)
                    {
                        if (h > 0 && currentHand[h] == currentHand[h - 1])
                        {
                            continue;
                        }

                        var nextBoard = Collapse(currentBoard.Insert(pos, currentHand[h].ToString()));
                        var nextHand = currentHand.Remove(h, 1);
                        var next = nextBoard + "|" + nextHand;

                        if (visited.TryAdd(next))
                        {
                            queue.Enqueue(next);
                        }
                    }
                }
            }

            moves++;
        }

        return -1;
    }

    private static string SortChars(string s)
    {
        var chars = s.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }

    private static string Collapse(string board)
    {
        while (true)
        {
            var next = CollapseOnePass(board);

            if (next == board)
            {
                return next;
            }

            board = next;
        }
    }

    private static string CollapseOnePass(string board)
    {
        var builder = new StringBuilder();
        var i = 0;

        while (i < board.Length)
        {
            var j = i;

            while (j < board.Length && board[j] == board[i])
            {
                j++;
            }

            if (j - i < 3)
            {
                builder.Append(board, i, j - i);
            }

            i = j;
        }

        return builder.ToString();
    }
}
