using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.MinimumMovesToSpreadStonesOverGrid;

// LeetCode 2850. Minimum Moves to Spread Stones Over Grid: a 3x3 grid holds 9
// stones total, distributed unevenly; one move slides one stone to an
// orthogonally adjacent cell, and the goal is the fewest moves to reach exactly
// one stone per cell. Because moves are single unit steps with no obstacles,
// the cost of relocating one stone from cell A to cell B by the shortest route
// is exactly their Manhattan distance, and - the key simplification - the
// total move count for ANY valid final routing equals the sum of Manhattan
// distances of SOME bijection between "excess" stones (cells with more than
// one) and "deficit" cells (cells with none), since stones from a
// non-excess/non-deficit cell never need to move at all. Minimizing total
// moves is therefore exactly the assignment problem: find the bijection
// minimizing summed Manhattan distance. With at most 8 excess stones (one cell
// holding all 9), brute-force permutation (<= 8! = 40320 leaves) is the
// standard accepted approach here - the grid is fixed-size 3x3, not a general
// n, so there is no larger n to grow into.
//
// MinimumMovesByBruteForcePermutation is the textbook recursive
// swap-and-recurse permutation walk (no repo primitive), evaluating every
// bijection's total cost directly - "what you'd write without this repo",
// mirroring MaximumEleganceOfAKLengthSubsequenceSolution's Bcl/RepoPrimitives
// contrast shape.
//
// MinimumMovesByBacktrackPermutation composes Algorithms.Backtracking.Backtrack
// - the exact same "Used[] boolean array + choose/unchoose over remaining
// indices" permutation walk the repo's own Permutations/PermutationsII
// coverage already proves out - threading the running Manhattan-distance sum
// through AssignmentState.Cost so Choose/Unchoose stay each other's exact
// inverse (Backtrack.cs's own precondition).
internal static class MinimumMovesToSpreadStonesOverGridSolution
{
    private const int GridSize = 3;

    public static int MinimumMovesByBruteForcePermutation(int[][] grid)
    {
        var (sources, targets) = BuildSourcesAndTargets(grid);

        if (sources.Count == 0)
        {
            return 0;
        }

        var used = new bool[sources.Count];
        var best = int.MaxValue;
        Permute(sources, targets, used, 0, 0, ref best);
        return best;
    }

    private static void Permute(
        List<(int Row, int Col)> sources, List<(int Row, int Col)> targets, bool[] used, int position, int costSoFar, ref int best)
    {
        if (position == sources.Count)
        {
            best = Math.Min(best, costSoFar);
            return;
        }

        for (var i = 0; i < sources.Count; i++)
        {
            if (used[i])
            {
                continue;
            }

            used[i] = true;
            Permute(sources, targets, used, position + 1, costSoFar + ManhattanDistance(sources[i], targets[position]), ref best);
            used[i] = false;
        }
    }

    public static int MinimumMovesByBacktrackPermutation(int[][] grid)
    {
        var (sources, targets) = BuildSourcesAndTargets(grid);

        if (sources.Count == 0)
        {
            return 0;
        }

        var best = int.MaxValue;
        var state = new AssignmentState(sources.Count);

        Backtrack.Search<AssignmentState, int>(
            state,
            st => st.Assigned.Count == sources.Count,
            st => st.Assigned.Count == sources.Count
                ? Enumerable.Empty<int>()
                : Enumerable.Range(0, sources.Count).Where(i => !st.Used[i]),
            (st, i) =>
            {
                st.Used[i] = true;
                st.Cost += ManhattanDistance(sources[i], targets[st.Assigned.Count]);
                st.Assigned.Add(i);
            },
            (st, i) =>
            {
                st.Assigned.RemoveAt(st.Assigned.Count - 1);
                st.Cost -= ManhattanDistance(sources[i], targets[st.Assigned.Count]);
                st.Used[i] = false;
            },
            st => best = Math.Min(best, st.Cost));

        return best;
    }

    private static (List<(int Row, int Col)> Sources, List<(int Row, int Col)> Targets) BuildSourcesAndTargets(int[][] grid)
    {
        var sources = new List<(int Row, int Col)>();
        var targets = new List<(int Row, int Col)>();

        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var excess = grid[row][col] - 1;

                if (excess > 0)
                {
                    for (var i = 0; i < excess; i++)
                    {
                        sources.Add((row, col));
                    }
                }
                else if (excess < 0)
                {
                    targets.Add((row, col));
                }
            }
        }

        return (sources, targets);
    }

    private static int ManhattanDistance((int Row, int Col) a, (int Row, int Col) b)
        => Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);

    private sealed class AssignmentState(int length)
    {
        public bool[] Used { get; } = new bool[length];

        public List<int> Assigned { get; } = [];

        public int Cost { get; set; }
    }
}
