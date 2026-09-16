using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignCircularQueueBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a wraparound array against this repo's own Deque -
// so a harness whose arms disagree is timing two different problems. Setup builds the whole call
// script from Capacity alone, so the same Capacity must rebuild the same script, and that script
// fills the queue exactly before every later operation runs at capacity.
public sealed partial class DesignCircularQueueBenchmarksTests
{
    private const int SmallestCapacity = 8;

    // Mirrors the benchmark's own script length: the replay's total is a closed form over it.
    private const int OperationCount = 50_000;

    [Fact]
    public void Setup_SameCapacity_RebuildsTheSameCallScript()
    {
        // The script enqueues one value per step, dequeues one only from the step where the queue
        // is already full, and reads Rear - the value just enqueued - after every step. That is a
        // total the test can compute from Capacity alone: every EnQueue succeeds, every step past
        // the fill has to make room first, and each Rear reports the step's own value.
        Assert.Equal(ExpectedReplayTotal(SmallestCapacity), BuildHarness().ArrayBacked());
        Assert.Equal(BuildHarness().ArrayBacked(), BuildHarness().ArrayBacked());
    }

    [Fact]
    public void ArrayBacked_FillThenWrapAroundChurn_AgreesWithDequeBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DequeBacked(), harness.ArrayBacked());
    }

    [Fact]
    public void DequeBacked_FillThenWrapAroundChurn_AgreesWithArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayBacked(), harness.DequeBacked());
    }

    private static DesignCircularQueueBenchmarks BuildHarness()
    {
        var harness = new DesignCircularQueueBenchmarks { Capacity = SmallestCapacity };
        harness.Setup();

        return harness;
    }

    // Every step's EnQueue contributes 1; every step from Capacity onwards contributes one more
    // for the DeQueue that makes room; and step i's Rear contributes the value i the step before
    // it enqueued.
    private static long ExpectedReplayTotal(int capacity) =>
        ((long)OperationCount * (OperationCount - 1) / 2) + OperationCount + (OperationCount - capacity);
}
