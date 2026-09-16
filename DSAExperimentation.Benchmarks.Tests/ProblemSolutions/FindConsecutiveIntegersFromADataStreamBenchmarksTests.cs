using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindConsecutiveIntegersFromADataStreamBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - an unbounded history rescan against a
// fixed-size Deque window with an incrementally maintained match count - so a harness whose arms
// disagree is replaying two different streams. Both arms return only a count of the true answers,
// which is a proxy: agreement witnesses that the two strategies agreed on every one of the streamed
// answers, but the count alone could also match on a stream where neither ever said true. Setup
// draws every arrival from [0, 1_000) and the window is 20 long with a single value that would fill
// it, so the documented "Value is rare in the stream" workload answers false throughout; the same
// Length must rebuild the same stream.
public sealed partial class FindConsecutiveIntegersFromADataStreamBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Twenty consecutive arrivals of the single tracked value would be needed to fill the window;
    // with every arrival drawn from [0, 1_000) that never happens over the seeded stream.
    private const int ExpectedConsecutiveRunCount = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameStream()
    {
        Assert.Equal(ExpectedConsecutiveRunCount, BuildHarness().UnboundedHistoryRescan());

        Assert.Equal(BuildHarness().UnboundedHistoryRescan(), BuildHarness().UnboundedHistoryRescan());
    }

    [Fact]
    public void UnboundedHistoryRescan_SeededStreamOfRareValue_AgreesWithFixedWindowIncrementalCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FixedWindowIncrementalCount(), harness.UnboundedHistoryRescan());
    }

    [Fact]
    public void FixedWindowIncrementalCount_SeededStreamOfRareValue_AgreesWithUnboundedHistoryRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnboundedHistoryRescan(), harness.FixedWindowIncrementalCount());
    }

    private static FindConsecutiveIntegersFromADataStreamBenchmarks BuildHarness()
    {
        var harness = new FindConsecutiveIntegersFromADataStreamBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
