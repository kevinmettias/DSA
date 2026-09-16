using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOperationsToEqualizeSubarraysBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the fewest element mutations that equalize
// each queried subarray - so a harness whose arms disagree is timing two different problems. Each arm
// answers every query in the fixed query list, in order, so the comparison is order-sensitive.
// MergeSortTree is handed the index [GlobalSetup] already built, so the comparison also pins that the
// hoisted merge-sort tree and run-id index describe the same nums the scan arm is given; Setup derives
// every query's answer from one seeded nums, so the same Length must rebuild the same workload.
public sealed partial class MinimumOperationsToEqualizeSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_SameQuerySet_AgreesWithMergeSortTree()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MergeSortTree()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void MergeSortTree_SameQuerySet_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.MergeSortTree()));
    }

    private static MinimumOperationsToEqualizeSubarraysBenchmarks BuildHarness()
    {
        var harness = new MinimumOperationsToEqualizeSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
