using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignCircularQueueBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a total derived from the script
// rather than from either arm. Setup builds the whole call script from Capacity alone, and that
// script fills the queue exactly before every later operation runs at capacity.
public sealed partial class DesignCircularQueueBenchmarksTests
{
    private const int SmallestCapacity = 8;

    // Mirrors the benchmark's own script length: the replay's total is a closed form over it.
    private const int OperationCount = 1_000;

    // The summed value of the closed form 1 + 2 + ... + (OperationCount - 1) is a triangular number,
    // which is that product over this.
    private const int TriangularNumberDivisor = 2;

    // The script enqueues one value per step, dequeues one only from the step where the queue is
    // already full, and reads Rear - the value just enqueued - after every step. That is a total the
    // test can compute from Capacity alone: every EnQueue succeeds, every step past the fill has to
    // make room first, and each Rear reports the step's own value.
    [Fact]
    public void ArrayBacked_FillThenWrapAroundChurn_SumsTheClosedFormTotal() =>
        Assert.Equal(ExpectedReplayTotal(SmallestCapacity), BuildHarness().ArrayBacked());

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
        ((long)OperationCount * (OperationCount - 1) / TriangularNumberDivisor) + OperationCount + (OperationCount - capacity);
}
