using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ValidArrangementOfPairsWorkloads (ARCHITECTURE 17.7). The reading depends on
// the pairs being an input LC 2097 could pose: no pair joins a node to itself, no two pairs are the
// same, and a valid arrangement exists. The trail is that arrangement - each pair starts where the one
// before it ended - and the shuffled pairs are the same pairs, so checking the trail chains and the
// shuffle keeps its pairs shows the arrangement exists without solving the problem. Both of
// ValidArrangementOfPairsBenchmarks' sizes are checked at its own seed and node count.
public sealed partial class ValidArrangementOfPairsWorkloadsTests
{
    // Mirrors ValidArrangementOfPairsBenchmarks' own private RandomSeed and NodeCount, and its sizes.
    private const int Seed = 2097;
    private const int NodeCount = 64;
    private const int SmallestPairCount = 200;
    private const int LargestPairCount = 2_000;

    public static TheoryData<int> PairCounts => new([SmallestPairCount, LargestPairCount]);

    [Theory]
    [MemberData(nameof(PairCounts))]
    public void BuildTrail_BenchmarkSizes_ChainsEachPairOntoTheLast(int pairCount)
    {
        var trail = ValidArrangementOfPairsWorkloads.BuildTrail(pairCount, NodeCount, Seed);

        Assert.Equal(pairCount, trail.Length);
        Assert.All(Enumerable.Range(1, pairCount - 1), i => Assert.Equal(trail[i - 1][1], trail[i][0]));
    }

    [Theory]
    [MemberData(nameof(PairCounts))]
    public void BuildTrail_BenchmarkSizes_HasDistinctPairsBetweenDistinctNodes(int pairCount)
    {
        var trail = ValidArrangementOfPairsWorkloads.BuildTrail(pairCount, NodeCount, Seed);

        Assert.All(trail, pair => Assert.NotEqual(pair[0], pair[1]));
        Assert.All(trail, pair => Assert.All(pair, node => Assert.InRange(node, 0, NodeCount - 1)));
        Assert.Equal(pairCount, trail.Select(pair => (pair[0], pair[1])).Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(PairCounts))]
    public void BuildPairs_BenchmarkSizes_ShufflesTheTrailsOwnPairs(int pairCount) =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(ValidArrangementOfPairsWorkloads.BuildTrail(pairCount, NodeCount, Seed)),
            AnswerGraphText.OfUnordered(ValidArrangementOfPairsWorkloads.BuildPairs(pairCount, NodeCount, Seed)));
}
