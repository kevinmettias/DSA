using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.Algorithms.Backtracking;

public sealed partial class BacktrackTests
{
    private sealed class SubsetsState
    {
        public List<int> ChosenIndices { get; } = [];
    }

    private sealed class QueensState(int size)
    {
        public int[] ColumnByRow { get; } = new int[size];

        public int Row { get; set; }
    }

    private sealed class DepthState
    {
        public int Depth { get; set; }
    }

    private sealed class PathState
    {
        public List<string> Path { get; } = ["A"];
    }

    // Chosen INDICES, not values, push/popped in Choose/Unchoose - the same
    // "push/pop instead of remember-the-previous-value" idiom that avoids needing
    // to separately save/restore a "next start index" field.
    [Fact]
    public void Search_Subsets_EnumeratesAllSubsetsInDfsOrder()
    {
        var nums = new[] { 1, 2, 3 };

        var subsets = EnumerateSubsetsInDfsOrder(nums);

        Assert.Equal(
            new[]
            {
                Array.Empty<int>(), new[] { 1 }, new[] { 1, 2 }, new[] { 1, 2, 3 },
                new[] { 1, 3 }, new[] { 2 }, new[] { 2, 3 }, new[] { 3 },
            },
            subsets);
    }

    private static List<int[]> EnumerateSubsetsInDfsOrder(int[] nums)
    {
        var state = new SubsetsState();
        var subsets = new List<int[]>();

        IEnumerable<int> CandidatesFrom(SubsetsState s)
        {
            var start = s.ChosenIndices.Count == 0 ? 0 : s.ChosenIndices[^1] + 1;
            return Enumerable.Range(start, nums.Length - start);
        }

        Backtrack.Search<SubsetsState, int>(
            state,
            isSolution: _ => true,
            candidates: CandidatesFrom,
            choose: (s, index) => s.ChosenIndices.Add(index),
            unchoose: (s, _) => s.ChosenIndices.RemoveAt(s.ChosenIndices.Count - 1),
            onSolution: s => subsets.Add(s.ChosenIndices.Select(i => nums[i]).ToArray()));

        return subsets;
    }

    [Fact]
    public void Search_NQueens_CountsFourByFourSolutionsAsTwo()
    {
        const int size = 4;
        var state = new QueensState(size);
        var solutionCount = 0;

        Backtrack.Search<QueensState, int>(
            state,
            isSolution: s => s.Row == size,
            candidates: s => Enumerable.Range(0, size).Where(col => IsSafeQueenColumn(s, col)),
            choose: (s, col) => { s.ColumnByRow[s.Row] = col; s.Row++; },
            unchoose: (s, _) => s.Row--,
            onSolution: _ => solutionCount++);

        Assert.Equal(2, solutionCount);
    }

    private static bool IsSafeQueenColumn(QueensState state, int col)
    {
        for (var row = 0; row < state.Row; row++)
        {
            var placedCol = state.ColumnByRow[row];

            if (placedCol == col || Math.Abs(placedCol - col) == state.Row - row)
            {
                return false;
            }
        }

        return true;
    }

    [Fact]
    // Only Row is asserted, not ColumnByRow's contents: Unchoose deliberately
    // leaves stale values past the current Row in place (the classic backtracking-
    // array idiom - IsSafeQueenColumn/Choose only ever read indices < Row, so a
    // stale trailing entry is never observed as live state). Row fully returning
    // to 0 is the real "search unwound completely" invariant.
    public void Search_NQueens_UnwindsRowToZeroAfterSearchCompletes()
    {
        const int size = 4;
        var state = new QueensState(size);

        Backtrack.Search<QueensState, int>(
            state,
            isSolution: s => s.Row == size,
            candidates: s => Enumerable.Range(0, size).Where(col => IsSafeQueenColumn(s, col)),
            choose: (s, col) => { s.ColumnByRow[s.Row] = col; s.Row++; },
            unchoose: (s, _) => s.Row--,
            onSolution: _ => { });

        Assert.Equal(0, state.Row);
    }

    // A depth-2, branching-factor-3 decision tree where every leaf "is a solution"
    // and OnSolution unconditionally accepts the first one reached.
    [Fact]
    public void TrySearch_OnSolutionReturningTrue_StopsSearchImmediately()
    {
        var visited = new List<string>();
        var state = new DepthState();

        var stopped = Backtrack.TrySearch(state, new BacktrackingSteps<DepthState, int>(
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
        var state = new DepthState();

        var stopped = Backtrack.TrySearch(state, new BacktrackingSteps<DepthState, int>(
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

        var visited = TraverseGraphDfsFromA(graph);

        Assert.Equal(new[] { "A", "B", "D", "C" }, visited);
    }

    private static List<string> TraverseGraphDfsFromA(Dictionary<string, string[]> graph)
    {
        var visited = new List<string> { "A" };
        var state = new PathState();

        Backtrack.Search<PathState, string>(
            state,
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
