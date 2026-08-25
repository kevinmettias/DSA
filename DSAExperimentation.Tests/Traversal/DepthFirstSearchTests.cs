using DSAExperimentation.Traversal;

namespace DSAExperimentation.Tests.Traversal;

public sealed partial class DepthFirstSearchTests
{
    // The essay's own "implicit graph" illustration: a chess-adjacent position, generated on
    // demand by a closure, no Graph type involved anywhere. A value-typed record struct
    // deliberately proves Traverse's TNode : notnull constraint (no `class`) is sufficient.
    private readonly record struct Position(int File, int Rank);

    [Fact]
    public void Traverse_CyclicSuccessors_VisitsEachNodeExactlyOnceAndTerminates()
    {
        var graph = new Dictionary<string, string[]>
        {
            ["A"] = ["B"],
            ["B"] = ["C"],
            ["C"] = ["A"],
        };

        var visited = DepthFirstSearch.Traverse("A", node => graph[node]);

        Assert.Equal(["A", "B", "C"], visited);
    }

    [Fact]
    public void Traverse_ImplicitGraphWithTransposition_VisitsEachPositionOnceDespiteSharedSuccessor()
    {
        // Two distinct move sequences ((0,0)->(1,0) and (0,0)->(0,1)->(1,0)) converge on the same
        // position - a transposition, the DAG-with-sharing shape the essay's chess example names.
        IEnumerable<Position> Moves(Position from) => from switch
        {
            { File: 0, Rank: 0 } => [new Position(1, 0), new Position(0, 1)],
            { File: 0, Rank: 1 } => [new Position(1, 0)],
            _ => [],
        };

        var visited = DepthFirstSearch.Traverse(new Position(0, 0), Moves);

        Assert.Equal([new Position(0, 0), new Position(1, 0), new Position(0, 1)], visited);
    }

    [Fact]
    public void Traverse_InfiniteSuccessorSpaceBoundedByCaller_StopsAtCallerImposedCutoff()
    {
        const int Cutoff = 5;

        IEnumerable<int> Increment(int n) => n < Cutoff ? [n + 1] : [];

        var visited = DepthFirstSearch.Traverse(0, Increment);

        Assert.Equal([0, 1, 2, 3, 4, 5], visited);
    }

    [Fact]
    public void Traverse_SelfLoop_VisitsNodeOnceAndDoesNotHang()
    {
        var visited = DepthFirstSearch.Traverse("A", _ => ["A"]);

        Assert.Equal(["A"], visited);
    }

    [Fact]
    public void Traverse_BranchingSuccessors_VisitsFirstListedSuccessorAndItsSubtreeBeforeTheSecond()
    {
        var graph = new Dictionary<string, string[]>
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = [],
            ["D"] = [],
        };

        var visited = DepthFirstSearch.Traverse("A", node => graph[node]);

        Assert.Equal(["A", "B", "D", "C"], visited);
    }

    [Fact]
    public void Traverse_WithCustomComparer_TreatsComparerEqualNodesAsAlreadyVisited()
    {
        IEnumerable<string> Successors(string node) => node switch
        {
            "a" => ["B"],
            "B" => ["A"],
            _ => [],
        };

        var visited = DepthFirstSearch.Traverse("a", Successors, StringComparer.OrdinalIgnoreCase);

        Assert.Equal(["a", "B"], visited);
    }
}
