using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfVisiblePeopleInAQueueBenchmarks (ARCHITECTURE 17.9): both arms return one
// visible-person count per position - the O(n^2) brute-force scan against the monotonic stack sweep -
// so a harness whose arms disagree is timing two different questions. The returned array is the
// problem's whole answer rather than a proxy, and its outer order is the queue order LeetCode's own
// return value pins, so the default order-sensitive rendering is the right comparison. Setup builds a
// seeded random permutation, so the same Length must rebuild the same heights.
public sealed partial class NumberOfVisiblePeopleInAQueueBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));

    [Fact]
    public void BruteForceScan_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void MonotonicStackSweep_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    private static NumberOfVisiblePeopleInAQueueBenchmarks BuildHarness()
    {
        var harness = new NumberOfVisiblePeopleInAQueueBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
