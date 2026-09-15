using System.Text;
using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;

namespace DSAExperimentation.LeetCode.ZumaGame;

// LeetCode 488. Zuma Game: insert one hand ball at a time into the board, then
// cascade-collapse any run of 3+ same-colored balls, until the board empties or the
// hand runs out. Both strategies search the same (board, remaining-hand) state
// space; they differ only in search order and how repeated states are handled.
internal static class ZumaGameSolution
{
    private const int MinCollapseRunLength = 3;
    private const char StateSeparator = '|';

    // The textbook baseline: branch-and-bound DFS over every (position, hand-ball)
    // insertion, cascade-collapse, recurse, pruned only by "already used at least as
    // many balls as the best solution found so far". Deliberately written without
    // this repo's primitives - it is the arm the BFS strategy below has to justify
    // itself against.
    public static int FindMinStepByBruteForceDfs(BallBoard board, BallHand hand)
    {
        var best = SearchMinSteps(new DfsSearchState(board.Text, hand.Text, 0, int.MaxValue));
        return best == int.MaxValue ? LeetCodeAnswer.None : best;
    }

    // This repo's own Queue<string> (BFS frontier) + Set<string> (visited-state
    // dedup) searching level by level, so the first level whose board empties out
    // holds the minimum number of balls needed - the exact
    // RemoveInvalidParentheses/LC301 shape.
    public static int FindMinStepByQueueBfsDedup(BallBoard board, BallHand hand)
    {
        var (visited, queue) = InitializeSearch(board, hand);

        return RunLevelOrderSearch(queue, visited);
    }

    private static (Set<string> Visited, RepoQueue Queue) InitializeSearch(BallBoard board, BallHand hand)
    {
        var visited = new Set<string>();
        var queue = new RepoQueue();
        var start = board.Text + StateSeparator + SortChars(hand.Text);
        visited.TryAdd(start);
        queue.Enqueue(start);

        return (visited, queue);
    }

    private static string SortChars(string s)
    {
        var chars = s.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }

    private static int RunLevelOrderSearch(RepoQueue queue, Set<string> visited)
    {
        var moves = 0;

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;

            for (var i = 0; i < levelSize; i++)
            {
                if (ProcessNextState(queue, visited))
                {
                    return moves;
                }
            }

            moves++;
        }

        return LeetCodeAnswer.None;
    }

    // Dequeues one (board, remaining-hand) state; returns true when the board is
    // already empty (the search is done), otherwise expands it into every reachable
    // next state and enqueues the not-yet-visited ones.
    private static bool ProcessNextState(RepoQueue queue, Set<string> visited)
    {
        queue.TryDequeue(out var state);
        var separator = state.IndexOf(StateSeparator);
        var currentBoard = new BallBoard(state[..separator]);
        var currentHand = new BallHand(state[(separator + 1)..]);

        if (currentBoard.Text.Length == 0)
        {
            return true;
        }

        ExpandState(currentBoard, currentHand, queue, visited);
        return false;
    }

    private static void ExpandState(
        BallBoard currentBoard, BallHand currentHand, RepoQueue queue, Set<string> visited)
    {
        for (var pos = 0; pos <= currentBoard.Text.Length; pos++)
        {
            for (var h = 0; h < currentHand.Text.Length; h++)
            {
                if (h > 0 && currentHand.Text[h] == currentHand.Text[h - 1])
                {
                    continue;
                }

                var placed = currentBoard.Text.Insert(pos, currentHand.Text[h].ToString());
                var nextBoard = Collapse(placed);
                var nextHand = currentHand.Text.Remove(h, 1);
                var next = nextBoard + StateSeparator + nextHand;

                if (visited.TryAdd(next))
                {
                    queue.Enqueue(next);
                }
            }
        }
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

    private static string Collapse(string board)
    {
        // Stops at the first cascade pass that changes nothing: no run of 3+ remains and the stable board is returned.
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

    private readonly record struct DfsSearchState(string Board, string Hand, int Used, int Best);
}
