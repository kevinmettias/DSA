using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.Algorithms.Backtracking;

// Each nested type is one scenario: the state a run of Backtrack works over, and - when the run is
// more than a call and an assertion - the script that drives it. The tests stay assertions, so a
// failure names the scenario that broke rather than a helper several screens away.
public sealed partial class BacktrackTests
{
    // The board both N-Queens examples search. A constant rather than a local in each
    // test: its scope is a claim about where the value is authoritative, and the
    // second reader of "4" is the other test, not the other branch.
    private const int Size = 4;

    [Fact]
    public void Search_Subsets_EnumeratesAllSubsetsInDfsOrder()
    {
        var subsets = new SubsetsScenario().EnumerateInDfsOrder(new[] { 1, 2, 3 });

        Assert.Equal(
            new[]
            {
                Array.Empty<int>(), new[] { 1 }, new[] { 1, 2 }, new[] { 1, 2, 3 },
                new[] { 1, 3 }, new[] { 2 }, new[] { 2, 3 }, new[] { 3 },
            },
            subsets);
    }

    [Fact]
    public void Search_NQueens_CountsFourByFourSolutionsAsTwo()
    {
        var board = new QueensScenario(Size);
        var solutionCount = 0;

        Backtrack.Search<QueensScenario, int>(
            board,
            isSolution: s => s.Row == Size,
            candidates: s => Enumerable.Range(0, Size).Where(col => s.IsSafeColumn(col)),
            choose: (s, col) => { s.ColumnByRow[s.Row] = col; s.Row++; },
            unchoose: (s, _) => s.Row--,
            onSolution: _ => solutionCount++);

        Assert.Equal(2, solutionCount);
    }

    [Fact]
    // Only Row is asserted, not ColumnByRow's contents: Unchoose deliberately
    // leaves stale values past the current Row in place (the classic backtracking-
    // array idiom - IsSafeColumn/Choose only ever read indices < Row, so a
    // stale trailing entry is never observed as live state). Row fully returning
    // to 0 is the real "search unwound completely" invariant.
    public void Search_NQueens_UnwindsRowToZeroAfterSearchCompletes()
    {
        var board = new QueensScenario(Size);

        Backtrack.Search<QueensScenario, int>(
            board,
            isSolution: s => s.Row == Size,
            candidates: s => Enumerable.Range(0, Size).Where(col => s.IsSafeColumn(col)),
            choose: (s, col) => { s.ColumnByRow[s.Row] = col; s.Row++; },
            unchoose: (s, _) => s.Row--,
            onSolution: _ => { });

        Assert.Equal(0, board.Row);
    }

    // A depth-2, branching-factor-3 decision tree where every leaf "is a solution"
    // and OnSolution unconditionally accepts the first one reached.
    [Fact]
    public void TrySearch_OnSolutionReturningTrue_StopsSearchImmediately()
    {
        var visited = new List<string>();
        var state = new DepthScenario();

        var stopped = Backtrack.TrySearch(state, new BacktrackingSteps<DepthScenario, int>(
            IsSolution: s => s.Depth == 2,
            Candidates: _ => new[] { 0, 1, 2 },
            Choose: (s, choice) =>
            {
                visited.Add($"{s.Depth}:{choice}");
                s.Depth++;
            },
            Unchoose: (s, _) => s.Depth--,
            OnSolution: _ => true));

        Assert.True(stopped);

        // Only the first branch at each level is ever tried: the depth-2 leaf
        // stops the whole search before any sibling candidate, at any level, is
        // ever attempted.
        Assert.Equal(new[] { "0:0", "1:0" }, visited);
    }

    [Fact]
    public void TrySearch_InitialStateAlreadyASolution_ReportsWithoutChoosingAnything()
    {
        var solutionCalls = 0;
        var state = new DepthScenario();

        var stopped = Backtrack.TrySearch(state, new BacktrackingSteps<DepthScenario, int>(
            IsSolution: _ => true,
            Candidates: _ => throw new InvalidOperationException("Candidates should not be consulted"),
            Choose: (_, _) => throw new InvalidOperationException("Choose should not be called"),
            Unchoose: (_, _) => throw new InvalidOperationException("Unchoose should not be called"),
            OnSolution: _ =>
            {
                solutionCalls++;
                return true;
            }));

        Assert.True(stopped);
        Assert.Equal(1, solutionCalls);
    }

    // Direct parity with DepthFirstSearchTests.Traverse_BranchingSuccessors_... -
    // isSolution always false, so this degrades to plain pre-order DFS with no
    // early exit, over the same A->[B,C], B->[D] graph shape.
    [Fact]
    public void Search_BranchingCandidates_VisitsFirstListedCandidateAndItsSubtreeBeforeTheSecond()
    {
        var graph = new Dictionary<string, string[]>
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = [],
            ["D"] = [],
        };

        var visited = new PathScenario().TraverseFromA(graph);

        Assert.Equal(new[] { "A", "B", "D", "C" }, visited);
    }

    /// <summary>
    /// The subsets scenario: its state, and the run that enumerates every subset in DFS order.
    /// Chosen INDICES, not values, are push/popped in Choose/Unchoose - the same "push/pop
    /// instead of remember-the-previous-value" idiom that avoids needing to separately
    /// save/restore a "next start index" field.
    /// </summary>
    private sealed record SubsetsScenario
    {
        public List<int> ChosenIndices { get; } = [];

        public List<int[]> EnumerateInDfsOrder(int[] nums)
        {
            var subsets = new List<int[]>();

            IEnumerable<int> CandidatesFrom(SubsetsScenario scenario)
            {
                var start = scenario.NextStartIndex();
                return Enumerable.Range(start, nums.Length - start);
            }

            Backtrack.Search<SubsetsScenario, int>(
                this,
                isSolution: _ => true,
                candidates: CandidatesFrom,
                choose: (scenario, index) => scenario.ChosenIndices.Add(index),
                unchoose: (scenario, _) => scenario.ChosenIndices.RemoveAt(scenario.ChosenIndices.Count - 1),
                onSolution: scenario => subsets.Add(scenario.ChosenIndices.Select(i => nums[i]).ToArray()));

            return subsets;
        }

        // The first candidate for a partial choice: one past the last index chosen, or
        // the whole range at the root, where nothing has been chosen yet.
        private int NextStartIndex()
        {
            if (ChosenIndices.Count == 0)
            {
                return 0;
            }

            return ChosenIndices[^1] + 1;
        }
    }

    /// <summary>
    /// The N-Queens scenario: the board the search places rows on - one column per row, plus how
    /// many rows are placed - and the safety test its Candidates callback reads.
    /// </summary>
    private sealed class QueensScenario(int size)
    {
        public int[] ColumnByRow { get; } = new int[size];

        public int Row { get; set; }

        // Safe when no already-placed row attacks the column: the same column, or a diagonal
        // (equal row and column distance). Only indices below Row are read, so the stale entries
        // Unchoose leaves behind are never observed as live state.
        public bool IsSafeColumn(int col)
        {
            for (var row = 0; row < Row; row++)
            {
                var placedCol = ColumnByRow[row];

                if (placedCol == col || Math.Abs(placedCol - col) == Row - row)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// The depth-bounded scenario's state: how deep into the open-ended decision tree the current
    /// branch has gone.
    /// </summary>
    private sealed class DepthScenario
    {
        public int Depth { get; set; }
    }

    /// <summary>
    /// The graph-walk scenario: the path from A the search is extending, and the run that walks
    /// A->[B,C], B->[D] in pre-order.
    /// </summary>
    private sealed class PathScenario
    {
        public List<string> Path { get; } = ["A"];

        public List<string> TraverseFromA(Dictionary<string, string[]> graph)
        {
            var visited = new List<string> { "A" };

            Backtrack.Search<PathScenario, string>(
                this,
                isSolution: _ => false,
                candidates: s => graph[s.Path[^1]],
                choose: (s, choice) =>
                {
                    s.Path.Add(choice);
                    visited.Add(choice);
                },
                unchoose: (s, _) => s.Path.RemoveAt(s.Path.Count - 1),
                onSolution: _ => { });

            return visited;
        }
    }
}
