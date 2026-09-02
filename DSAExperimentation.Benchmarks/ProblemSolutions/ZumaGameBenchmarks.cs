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
    private const string BoardRepeatUnit = "RRWW";
    private const string StateSeparator = "|";
    private const int MinCollapseRunLength = 3;

    [Params(3, 9)]
    public int BoardRepeats;

    private string _board = null!;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedSegments = Enumerable.Repeat(BoardRepeatUnit, BoardRepeats);
        _board = string.Concat(repeatedSegments);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDfs() => FindMinStepDfs(_board, Hand);

    [Benchmark]
    public int QueueBfsDedup() => FindMinStepBfs(_board, Hand);

    private static int FindMinStepDfs(string board, string hand)
    {
        var best = SearchMinSteps(new DfsSearchState(board, hand, 0, int.MaxValue));
        return best == int.MaxValue ? -1 : best;
    }

    private static int SearchMinSteps(DfsSearchState state)
    {
        if (state.Board.Length == 0)
        {
            return Math.Min(state.Best, state.Used);
        }

        if (state.Hand.Length == 0 || state.Used >= state.Best)
        {
            return state.Best;
        }

        var best = state.Best;

        for (var pos = 0; pos <= state.Board.Length; pos++)
        {
            for (var h = 0; h < state.Hand.Length; h++)
            {
                best = TryInsertAndRecurse(pos, h, state with { Best = best });
            }
        }

        return best;
    }

    private static int TryInsertAndRecurse(int pos, int h, DfsSearchState state)
    {
        var inserted = state.Board.Insert(pos, state.Hand[h].ToString());
        var nextBoard = Collapse(inserted);
        var nextHand = state.Hand.Remove(h, 1);
        return SearchMinSteps(state with { Board = nextBoard, Hand = nextHand, Used = state.Used + 1 });
    }

    private readonly record struct DfsSearchState(string Board, string Hand, int Used, int Best);

    private static int FindMinStepBfs(string board, string hand)
    {
        var (visited, queue) = InitializeBfsState(board, hand);
        return RunBfs(queue, visited);
    }

    private static (Set<string> Visited, RepoQueue Queue) InitializeBfsState(string board, string hand)
    {
        var visited = new Set<string>();
        var queue = new RepoQueue();
        var start = board + StateSeparator + SortChars(hand);
        visited.TryAdd(start);
        queue.Enqueue(start);
        return (visited, queue);
    }

    private static int RunBfs(RepoQueue queue, Set<string> visited)
    {
        var moves = 0;

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;

            for (var i = 0; i < levelSize; i++)
            {
                var result = TryDequeueLevelState(queue, visited, moves);

                if (result.HasValue)
                {
                    return result.Value;
                }
            }

            moves++;
        }

        return -1;
    }

    private static int? TryDequeueLevelState(RepoQueue queue, Set<string> visited, int moves)
    {
        queue.TryDequeue(out var state);
        var separator = state.IndexOf('|');
        var currentBoard = state[..separator];
        var currentHand = state[(separator + 1)..];

        if (currentBoard.Length == 0)
        {
            return moves;
        }

        ExpandNextStates(currentBoard, currentHand, queue, visited);
        return null;
    }

    private static void ExpandNextStates(string currentBoard, string currentHand, RepoQueue queue, Set<string> visited)
    {
        for (var pos = 0; pos <= currentBoard.Length; pos++)
        {
            for (var h = 0; h < currentHand.Length; h++)
            {
                if (h > 0 && currentHand[h] == currentHand[h - 1])
                {
                    continue;
                }

                var inserted = currentBoard.Insert(pos, currentHand[h].ToString());
                var nextBoard = Collapse(inserted);
                var nextHand = currentHand.Remove(h, 1);
                var next = nextBoard + StateSeparator + nextHand;

                if (visited.TryAdd(next))
                {
                    queue.Enqueue(next);
                }
            }
        }
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

            if (j - i < MinCollapseRunLength)
            {
                builder.Append(board, i, j - i);
            }

            i = j;
        }

        return builder.ToString();
    }
}
