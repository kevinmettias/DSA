using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignFrontMiddleBackQueueBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - one List<int> inserting at all three positions
// against the two-deque split - so a harness whose arms disagree is timing two different
// problems. There is no [GlobalSetup]: each arm constructs its own queue and runs the same
// Calls-length push cycle, which the smallest Calls drives far enough to shift a large list.
public sealed partial class DesignFrontMiddleBackQueueBenchmarksTests
{
    private const int SmallestCalls = 5_000;

    // Both arms return the queue's own Count rather than an element, so agreement pins that both
    // ran the same push cycle to completion and no more - the return carries no per-push detail
    // for a stronger claim to rest on.
    [Fact]
    public void ArrayListInsertAtPosition_FrontMiddleBackPushCycle_AgreesWithTwoDequeFrontMiddleBackQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoDequeFrontMiddleBackQueue(), harness.ArrayListInsertAtPosition());
    }

    [Fact]
    public void TwoDequeFrontMiddleBackQueue_FrontMiddleBackPushCycle_AgreesWithArrayListInsertAtPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayListInsertAtPosition(), harness.TwoDequeFrontMiddleBackQueue());
    }

    private static DesignFrontMiddleBackQueueBenchmarks BuildHarness() => new() { Calls = SmallestCalls };
}
