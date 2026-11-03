using DSAExperimentation.LeetCode.Solutions.RaceCar;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RaceCar;

// Harness only. Both strategies - the textbook mutate-the-tuple BFS and this repo's
// Reduce.Graph over the materialized command graph - are RaceCarSolution's; this
// file states LeetCode's published examples once, plus further targets whose
// shortest command sequences exercise overshoot-and-reverse rather than a single
// run of 'A's.
public sealed partial class RaceCarSolutionTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            // LC example 1: "AA" overshoots, so "A" alone is the answer.
            { 1, 1 },

            // LC example 2: "AAR" lands on 3 then reverses past, "AARA" is 4 commands.
            { 2, 4 },

            // "AA" reaches 3 exactly.
            { 3, 2 },

            // Overshoot to 7, reverse, and crawl back.
            { 4, 5 },

            { 5, 7 },

            // LC's own worked example: "AAARA" - 0 -> 1 -> 3 -> 7, reverse, 6.
            { 6, 5 },

            // A power-of-two-minus-one target: three 'A's and nothing else.
            { 7, 3 },

            { 8, 6 },

            { 10, 7 },

            // Large enough that the reversal has to happen more than once.
            { 25, 11 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCommandsByMutationQueue_LeetCodeExamples_ReturnsShortestCommandCount(int target, int expected) =>
        Assert.Equal(expected, RaceCarSolution.MinCommandsByMutationQueue(target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCommandsByReduceGraph_LeetCodeExamples_ReturnsShortestCommandCount(int target, int expected) =>
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
