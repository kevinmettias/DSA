using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SequentiallyOrdinalRankTrackerBenchmarks (ARCHITECTURE 17.9): both arms replay
// the identical alternating add/get script against their own tracker, so a harness whose arms
// disagree is timing two different scripts. Each arm constructs its mutable subject inside the call -
// the re-sort arm its own tracker, the two-heap arm its own - so one harness instance is safe to call
// twice in either order, and the name and score fields it reads are never written by either arm.
// Setup draws both from one fixed seed, so the same OperationCount must rebuild the same script.
public sealed partial class SequentiallyOrdinalRankTrackerBenchmarksTests
{
    private const int SmallestOperationCount = 100;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ResortEveryGet()),
            AnswerText.Of(BuildHarness().ResortEveryGet()));

    [Fact]
    public void ResortEveryGet_AlternatingAddAndGet_AgreesWithTwoHeapTracker()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TwoHeapTracker()),
            AnswerText.Of(harness.ResortEveryGet()));
    }

    [Fact]
    public void TwoHeapTracker_AlternatingAddAndGet_AgreesWithResortEveryGet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ResortEveryGet()),
            AnswerText.Of(harness.TwoHeapTracker()));
    }

    private static SequentiallyOrdinalRankTrackerBenchmarks BuildHarness()
    {
        var harness = new SequentiallyOrdinalRankTrackerBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
