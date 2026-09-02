using DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToTransportAllIndividuals;

// Harness only. The round-generation rule is TransportGraph's and both search
// strategies are MinimumTimeToTransportAllIndividualsSolution's - this file
// just pins them to LeetCode's published examples, including the k = 1 case
// that never reaches a full mask at all.
public sealed class MinimumTimeToTransportAllIndividualsTests
{
    public static TheoryData<int[], int, double[], double> Examples =>
        new()
        {
            { [5], 1, [1.0, 1.3], 5.0 },
            { [2, 5, 8], 2, [1.0, 1.5, 0.75], 14.5 },
            { [10, 10], 1, [2.0, 2.0], -1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByBruteForceDijkstra_LeetCodeExamples_ReturnsMinimumCrossingTime(
        int[] time, int capacity, double[] mul, double expected) =>
        Assert.Equal(
            expected,
            MinimumTimeToTransportAllIndividualsSolution.MinTimeByBruteForceDijkstra(time, capacity, mul));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByDijkstraOverTransportGraph_LeetCodeExamples_ReturnsMinimumCrossingTime(
        int[] time, int capacity, double[] mul, double expected) =>
        Assert.Equal(
            expected,
            MinimumTimeToTransportAllIndividualsSolution.MinTimeByDijkstraOverTransportGraph(time, capacity, mul));
}
