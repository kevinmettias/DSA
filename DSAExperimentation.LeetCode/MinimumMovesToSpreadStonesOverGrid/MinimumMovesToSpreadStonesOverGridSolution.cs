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
        Permute((sources, targets), (used, 0, 0), ref best);
        return best;
    }

    public static int MinimumMovesByBacktrackPermutation(int[][] grid)
    {
        var (sources, targets) = BuildSourcesAndTargets(grid);

        if (sources.Count == 0)
        {
            return 0;
        }

        return RunBacktrackSearch(sources, targets);
    }

    // The composed arm's walk: AssignmentState threads the running Manhattan-distance
    // sum through Choose/Unchoose so the pair stays each other's exact inverse, and the
    // best cost is folded in on each complete assignment and returned. It is a local
    // rather than a ref because the fold runs inside the solution callback, and C#
    // forbids a lambda capturing a ref parameter.
    private static int RunBacktrackSearch(
        List<(int Row, int Col)> sources, List<(int Row, int Col)> targets)
    {
        var state = new AssignmentState(sources.Count);
        var best = int.MaxValue;

        Backtrack.Search<AssignmentState, int>(
            state,
            st => st.Assigned.Count == sources.Count,
            st => st.Assigned.Count == sources.Count
                ? Enumerable.Empty<int>()
                : UnassignedSources(st, sources),
            (st, i) => ChooseSource(st, sources, targets, i),
            (st, i) => UnchooseSource(st, sources, targets, i),
            st => best = Math.Min(best, st.Cost));

        return best;
    }

    // Every source stone still unassigned, in index order - the moves the walk may
    // choose next.
    private static IEnumerable<int> UnassignedSources(
        AssignmentState state, List<(int Row, int Col)> sources) =>
        Enumerable.Range(0, sources.Count).Where(i => !state.Used[i]);

    // Source i takes the position the walk is currently filling: it is marked used,
    // charged the Manhattan distance to the target that position owns, and recorded
    // as the pick.
    private static void ChooseSource(
        AssignmentState state,
        List<(int Row, int Col)> sources,
        List<(int Row, int Col)> targets,
        int index)
    {
        state.Used[index] = true;
        state.Cost += ManhattanDistance(sources[index], targets[state.Assigned.Count]);
        state.Assigned.Add(index);
    }

    // UnchooseSource is ChooseSource's exact inverse - the pick comes back off, the
    // distance it charged is given back, and the source is free again - which is the
    // precondition Backtrack.cs's walk rests on.
    private static void UnchooseSource(
        AssignmentState state,
        List<(int Row, int Col)> sources,
        List<(int Row, int Col)> targets,
        int index)
    {
        state.Assigned.RemoveAt(state.Assigned.Count - 1);
        state.Cost -= ManhattanDistance(sources[index], targets[state.Assigned.Count]);
        state.Used[index] = false;
    }

    // The excess-stone cells and the deficit cells are the two sides of one bijection,
    // and a position plus the cost accumulated to reach it is exactly the state
    // AssignmentState threads through Choose/Unchoose for the composed arm - so each
    // pair travels as one argument. The best cost stays a ref: it is the one value
    // every path in the search shares.
    private static void Permute(
        (List<(int Row, int Col)> Sources, List<(int Row, int Col)> Targets) assignment,
        (bool[] Used, int Position, int Cost) state,
        ref int best)
    {
        if (state.Position == assignment.Sources.Count)
        {
            best = Math.Min(best, state.Cost);
            return;
        }

        for (var i = 0; i < assignment.Sources.Count; i++)
        {
            TryAssignSource(assignment, state, i, ref best);
        }
    }

    // One branch of the walk: source i takes the current position, the cost of that
    // pairing is added, and the extended state recurses - the choose/recurse/unchoose
    // step the loop above repeats over every source still free.
    private static void TryAssignSource(
        (List<(int Row, int Col)> Sources, List<(int Row, int Col)> Targets) assignment,
        (bool[] Used, int Position, int Cost) state,
        int index,
        ref int best)
    {
        if (state.Used[index])
        {
            return;
        }

        state.Used[index] = true;

        var next = (
            Used: state.Used,
            Position: state.Position + 1,
            Cost: state.Cost + ManhattanDistance(assignment.Sources[index], assignment.Targets[state.Position]));

        Permute(assignment, next, ref best);
        state.Used[index] = false;
    }

    private static (List<(int Row, int Col)> Sources, List<(int Row, int Col)> Targets) BuildSourcesAndTargets(int[][] grid)
    {
        var sources = new List<(int Row, int Col)>();
        var targets = new List<(int Row, int Col)>();

        ClassifyCells(grid, sources, targets);

        return (sources, targets);
    }

    // Each cell is one side of the bijection or neither: a cell holding more than its
    // one stone contributes that many interchangeable sources, a cell holding none is
    // a target, and a cell holding exactly one never moves.
    private static void ClassifyCells(
        int[][] grid, List<(int Row, int Col)> sources, List<(int Row, int Col)> targets)
    {
        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var excess = grid[row][col] - 1;

                if (excess > 0)
                {
                    AddExcessStones(sources, (row, col), excess);
                }
                else if (excess < 0)
                {
                    targets.Add((row, col));
                }
            }
        }
    }

    // An excess cell's surplus stones are that many sources standing in the same place.
    private static void AddExcessStones(
        List<(int Row, int Col)> sources, (int Row, int Col) cell, int excess)
    {
        for (var i = 0; i < excess; i++)
        {
            sources.Add(cell);
        }
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
