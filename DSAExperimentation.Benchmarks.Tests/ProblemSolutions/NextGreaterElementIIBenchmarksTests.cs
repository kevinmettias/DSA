using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NextGreaterElementIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) scan-ahead baseline against the O(n) monotonic-stack
// sweep over this repo's own Stack<int> - so a harness whose arms disagree is answering two
// different circular queries. Setup draws the seeded value array, so the same Length must rebuild
// the same values.
//
// The values repeat (they are drawn from a bounded range, unlike the sibling problem's permutation),
// so the sweep's per-index pop/push bookkeeping is exercised on real ties rather than on strictly
// distinct values alone. Neither arm mutates the array, so one harness serves both in either order.
public sealed partial class NextGreaterElementIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStack()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void MonotonicStack_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.MonotonicStack()));
    }

    private static NextGreaterElementIIBenchmarks BuildHarness()
    {
        var harness = new NextGreaterElementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
