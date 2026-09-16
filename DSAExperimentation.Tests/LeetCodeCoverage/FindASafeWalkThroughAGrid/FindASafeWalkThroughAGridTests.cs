using DSAExperimentation.LeetCode.FindASafeWalkThroughAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindASafeWalkThroughAGrid;

// Harness only: both strategies are FindASafeWalkThroughAGridSolution's - this
// file just pins them to LeetCode's published examples, including the one where
// every path except a single detour is unsafe.
public sealed class FindASafeWalkThroughAGridTests
{
    public static TheoryData<SafeWalkCase> Examples =>
        new()
        {
            { new SafeWalkCase([[0, 1, 0, 0, 0], [0, 1, 0, 1, 0], [0, 0, 0, 1, 0]], 1, IsSafe: true) },
            {
                new SafeWalkCase(
                    [[0, 1, 1, 0, 0, 0], [1, 0, 1, 0, 0, 0], [0, 1, 1, 1, 0, 1], [0, 0, 1, 0, 1, 0]],
                    3,
                    IsSafe: false)
            },
            { new SafeWalkCase([[1, 1, 1], [1, 0, 1], [1, 1, 1]], 5, IsSafe: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByBruteForceArrayDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        SafeWalkCase example)
    {
        var actual = FindASafeWalkThroughAGridSolution.IsSafeByBruteForceArrayDijkstra(
            example.Grid, example.Health);

        Assert.Equal(example.IsSafe, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByWeightedGridDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        SafeWalkCase example)
    {
        var actual = FindASafeWalkThroughAGridSolution.IsSafeByWeightedGridDijkstra(
            example.Grid, example.Health);

        Assert.Equal(example.IsSafe, actual);
    }

    // One LeetCode example: the grid walked, the starting health, and whether a route to
    // the last row and column keeps the health positive all the way. The expected value
    // is named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct SafeWalkCase(int[][] Grid, int Health, bool IsSafe);
}
