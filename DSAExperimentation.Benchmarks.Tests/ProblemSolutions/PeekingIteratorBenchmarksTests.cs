using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeekingIteratorBenchmarks (ARCHITECTURE 17.9): its two arms are
// PeekingIteratorSolution's, competing backings for the same peek-peek-next contract - a raw index
// plus buffered value against the repo's own queue, whose peek-and-dequeue pair is already the
// contract - so a harness whose arms disagree is timing two different problems. Each arm builds its
// own iterator inside the call, so one harness is safe to call in either order. Setup builds the
// source sequence from Length alone, and the drain total each arm sums is fixed by that sequence,
// so the tests pin the total as well as the agreement.
public sealed partial class PeekingIteratorBenchmarksTests
{
    private const int SmallestLength = 200;

    // Drain sums Peek and Next on every element, so it adds each of 0..Length-1 twice.
    private const long ExpectedDrainedTotal = (long)SmallestLength * (SmallestLength - 1);

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().IndexTracked(), BuildHarness().IndexTracked());

    [Fact]
    public void IndexTracked_SequentialValues_AgreesWithQueuePrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDrainedTotal, harness.QueuePrimitive());
        Assert.Equal(harness.QueuePrimitive(), harness.IndexTracked());
    }

    [Fact]
    public void QueuePrimitive_SequentialValues_AgreesWithIndexTracked()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDrainedTotal, harness.IndexTracked());
        Assert.Equal(harness.IndexTracked(), harness.QueuePrimitive());
    }

    private static PeekingIteratorBenchmarks BuildHarness()
    {
        var harness = new PeekingIteratorBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
