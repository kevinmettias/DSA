using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumXORWithAnElementFromArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning nums per query against the offline sweep that
// sorts the limit-and-xor pairs and walks one BitTrie - so a harness whose arms disagree is timing two
// different problems. Both arms return one answer per query in query order, which is the order LeetCode
// pins, so AnswerText.Of's order-sensitive rendering is the right comparison. Neither arm mutates nums
// or the query pairs, so one harness is safe to read twice in either order; Setup draws both from one
// fixed seed, so the same Length must rebuild the same nums and the same queries.
public sealed partial class MaximumXORWithAnElementFromArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanPerQuery()),
            AnswerText.Of(BuildHarness().LinearScanPerQuery()));

    [Fact]
    public void LinearScanPerQuery_SeededNumsAndQueries_AgreesWithOfflineSortedBitTrieSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.OfflineSortedBitTrieSweep()),
            AnswerText.Of(harness.LinearScanPerQuery()));
    }

    [Fact]
    public void OfflineSortedBitTrieSweep_SeededNumsAndQueries_AgreesWithLinearScanPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanPerQuery()),
            AnswerText.Of(harness.OfflineSortedBitTrieSweep()));
    }

    private static MaximumXORWithAnElementFromArrayBenchmarks BuildHarness()
    {
        var harness = new MaximumXORWithAnElementFromArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
