using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
// (ARCHITECTURE 17.9): both arms answer the same question - the length of a qualifying window
// in LC 2334, or LeetCodeAnswer.None - one by a window-minimum scan, one by a DisjointSet sweep
// in descending value order, so a harness whose arms disagree is timing two different problems.
// Setup's values are seeded, so the same length must rebuild the same workload.
//
// The fixture's threshold is deliberately unreachable, so both arms are forced through their
// full scan and both must report the no-valid-subarray answer: agreement alone would also hold
// for two arms that both returned something else, which is why the expectation is asserted too.
public sealed partial class SubarrayWithElementsGreaterThanVaryingThresholdBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().WindowMinimumScan()),
            AnswerText.Of(BuildHarness().WindowMinimumScan()));

    [Fact]
    public void WindowMinimumScan_AgreesWithUnionFindOrder()
    {
        var harness = BuildHarness();
        var windowScan = harness.WindowMinimumScan();

        Assert.Equal(windowScan, harness.UnionFindOrder());
        Assert.Equal(LeetCodeAnswer.None, windowScan);
    }

    [Fact]
    public void UnionFindOrder_AgreesWithWindowMinimumScan()
    {
        var harness = BuildHarness();
        var unionFind = harness.UnionFindOrder();

        Assert.Equal(unionFind, harness.WindowMinimumScan());
        Assert.Equal(LeetCodeAnswer.None, unionFind);
    }

    private static SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks BuildHarness()
    {
        var harness = new SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
        {
            Length = SmallestLength,
        };

        harness.Setup();

        return harness;
    }
}
