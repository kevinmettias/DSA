using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfKConsecutiveBitFlipsBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumNumberOfKConsecutiveBitFlipsSolution's, the same methods
// MinimumNumberOfKConsecutiveBitFlipsTests proves correct, and both run the same greedy over
// the same bit array - they differ only in how the current bit's cumulative flip parity is
// learned - so arms that disagree are timing two different problems.
//
// The parity-tracking arm never touches the array and the in-place arm clones it before
// rewriting, so one harness instance answers both arms without either seeing the other's work.
public sealed partial class MinimumNumberOfKConsecutiveBitFlipsBenchmarksTests
{
    // The smallest declared [Params] value: the in-place arm rewrites a window on every
    // uncovered zero, so a shorter bit array is the cheaper way to reach the same comparison.
    private const int SmallestLength = 3_000;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().InPlaceWindowFlip(),
            BuildHarness().InPlaceWindowFlip());

    [Fact]
    public void InPlaceWindowFlip_AgreesWithQueueTrackedParity()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueTrackedParity(), harness.InPlaceWindowFlip());
    }

    [Fact]
    public void QueueTrackedParity_AgreesWithInPlaceWindowFlip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InPlaceWindowFlip(), harness.QueueTrackedParity());
    }

    private static MinimumNumberOfKConsecutiveBitFlipsBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfKConsecutiveBitFlipsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
