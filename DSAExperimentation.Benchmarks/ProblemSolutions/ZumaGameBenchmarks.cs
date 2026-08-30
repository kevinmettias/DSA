using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Zuma Game (LC 488): the branch-and-bound DFS most people reach for first - try
// every (position, hand-ball) insertion, cascade-collapse, recurse, pruned only by
// "already used at least as many balls as the best solution found so far" - vs. this
// repo's own Queue<string> (BFS frontier) + Set<string> (visited-state dedup) doing
// the same level-by-level search ZumaGameTests.cs uses. _board repeats "RRWW" (no run
// >=3 initially, per LC488's own precondition), so different insertion positions
// inside the same "WW"/"RR" run genuinely collapse to the identical resulting state -
// exactly the redundancy BruteForceDfs keeps re-exploring and QueueBfsDedup's
// Set<string> skips after the first time. BoardRepeats is kept odd at both sizes
// (this pattern happens to admit a same-move-count shortcut solution whenever the
// repeat count is even) so both sizes need the same 3-ball solution and the naive
// baseline's growth reflects board size, not a smaller answer getting lucky.
[MemoryDiagnoser]
public class ZumaGameBenchmarks
{
    private const string Hand = "WWWWW";

    [Params(3, 9)]
    public int BoardRepeats;

    private string _board = null!;

    [GlobalSetup]
    public void Setup() => _board = string.Concat(Enumerable.Repeat("RRWW", BoardRepeats));

    [Benchmark(Baseline = true)]
    public int BruteForceDfs() => FindMinStepDfs(_board, Hand);

    [Benchmark]
    public int QueueBfsDedup() => FindMinStepBfs(_board, Hand);

    private static int FindMinStepDfs(string board, string hand)
    {
        var best = int.MaxValue;
        Search(board, hand, 0);
        return best == int.MaxValue ? -1 : best;

        void Search(string currentBoard, string currentHand, int used)
        {
            if (currentBoard.Length == 0)
            {
                best = Math.Min(best, used);
                return;
            }

            if (currentHand.Length == 0 || used >= best)
            {
                return;
            }

            for (var pos = 0; pos <= currentBoard.Length; pos++)
            {
                for (var h = 0; h < currentHand.Length; h++)
                {
                    var nextBoard = Collapse(currentBoard.Insert(pos, currentHand[h].ToString()));
                    var nextHand = currentHand.Remove(h, 1);
                    Search(nextBoard, nextHand, used + 1);
                }
            }
        }
    }

    private static int FindMinStepBfs(string board, string hand)
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
