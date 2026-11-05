using DSAExperimentation.LeetCode.Solutions.RaceCar;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RaceCar;

// Harness only. Both strategies - the textbook mutate-the-tuple BFS and this repo's
// Reduce.Graph over the materialized command graph - are RaceCarSolution's; this
// file states targets whose shortest command sequences exercise overshoot-and-reverse
// rather than a single run of 'A's. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class RaceCarSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            // "AAR" lands on 3 then reverses past, "AARA" is 4 commands.
            { 2, 4 },

            // Overshoot to 3, reverse back to 2, then reverse again: "AARARAA" is 7 commands.
            { 5, 7 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCommandsByMutationQueue_Cases_ReturnsShortestCommandCount(int target, int expected) =>
        Assert.Equal(expected, RaceCarSolution.MinCommandsByMutationQueue(target));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCommandsByReduceGraph_Cases_ReturnsShortestCommandCount(int target, int expected) =>
        Assert.Equal(expected, RaceCarSolution.MinCommandsByReduceGraph(target));

    // Target 3's box runs from -14 to 14, with speeds of magnitude 1 to 16: 29 positions of 10 speeds.
    [Fact]
    public void BuildCommandGraph_HoldsEveryStateInTheTargetsBox() =>
        Assert.Equal(290, RaceCarSolution.BuildCommandGraph(3).Count);

    [Fact]
    public void BuildCommandGraph_SpansTheBoxEvenlyEitherSideOfZero()
    {
        var graph = RaceCarSolution.BuildCommandGraph(3);

        Assert.True(graph.TryGetNode((-14, 1), out _));
        Assert.True(graph.TryGetNode((14, -16), out _));
        Assert.False(graph.TryGetNode((15, 1), out _));
        Assert.False(graph.TryGetNode((-15, 1), out _));
    }

    // From the start, 'A' moves to (1, 2) and 'R' turns the car round in place.
    [Fact]
    public void BuildCommandGraph_JoinsAStateByItsTwoCommands()
    {
        var graph = RaceCarSolution.BuildCommandGraph(3);

        Assert.True(graph.TryGetNode((0, 1), out var start));
        Assert.Equal([(1, 2), (0, -1)], start.Successors.Select(successor => successor.State));
    }

    // An 'A' that would leave the box lands on no state, so the graph keeps only the 'R'.
    [Fact]
    public void BuildCommandGraph_LeavesOutAnAccelerationThatLeavesTheBox()
    {
        var graph = RaceCarSolution.BuildCommandGraph(3);

        Assert.True(graph.TryGetNode((14, 16), out var edge));
        Assert.Equal([(14, -1)], edge.Successors.Select(successor => successor.State));
    }
}
