using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfPairsAfterIncrementBenchmarks (ARCHITECTURE 17.9): both arms replay the
// same query stream and return one count per count-query - the direct array pass against the range
// Fenwick tree - so a harness whose arms disagree is timing two different questions. The returned
// array is the problem's whole answer rather than a proxy, and its outer order is pinned by the query
// stream itself, so the default order-sensitive rendering is the right comparison. Setup builds
// nums1, nums2 and the query stream, so the same Nums2Length must rebuild all three.
public sealed partial class NumberOfPairsAfterIncrementBenchmarksTests
{
    private const int SmallestNums2Length = 500;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DirectArray()),
            AnswerText.Of(BuildHarness().DirectArray()));

    [Fact]
    public void DirectArray_AgreesWithRangeFenwickTree()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RangeFenwickTree()),
            AnswerText.Of(harness.DirectArray()));
    }

    [Fact]
    public void RangeFenwickTree_AgreesWithDirectArray()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DirectArray()),
            AnswerText.Of(harness.RangeFenwickTree()));
    }

    private static NumberOfPairsAfterIncrementBenchmarks BuildHarness()
    {
        var harness = new NumberOfPairsAfterIncrementBenchmarks { Nums2Length = SmallestNums2Length };
        harness.Setup();

        return harness;
    }
}
