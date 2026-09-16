using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NextGreaterElementIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the per-query rescan against a single monotonic-stack sweep -
// so a harness whose arms disagree is answering two different queries. Setup draws both arrays from
// the seeded permutation, so the same Length must rebuild the same nums1 and nums2 in the same order.
//
// Neither arm writes to the arrays it is handed (the sweep builds its own answer array), so one
// harness serves both arms in either order, and the per-element agreement is pinned by rendering
// each returned array rather than by reference equality.
public sealed partial class NextGreaterElementIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerQueryRescan()),
            AnswerText.Of(BuildHarness().PerQueryRescan()));

    [Fact]
    public void MonotonicStackSweep_AgreesWithPerQueryRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerQueryRescan()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    [Fact]
    public void PerQueryRescan_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.PerQueryRescan()));
    }

    private static NextGreaterElementIBenchmarks BuildHarness()
    {
        var harness = new NextGreaterElementIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
