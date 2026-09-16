using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveKDigitsBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveKDigitsSolution's, competing strategies for the same question - repeatedly deleting the first
// descent against one monotonic-stack sweep - so a harness whose arms disagree returns two different
// smallest numbers. Setup draws the digits from a fixed Random and derives the removal count from
// Length, so the same Length must rebuild the same (num, removals) pair.
public sealed partial class RemoveKDigitsBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().RepeatedFirstDescentRemoval(),
            BuildHarness().RepeatedFirstDescentRemoval());

    [Fact]
    public void RepeatedFirstDescentRemoval_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackSweep(), harness.RepeatedFirstDescentRemoval());
    }

    [Fact]
    public void MonotonicStackSweep_AgreesWithRepeatedFirstDescentRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedFirstDescentRemoval(), harness.MonotonicStackSweep());
    }

    private static RemoveKDigitsBenchmarks BuildHarness()
    {
        var harness = new RemoveKDigitsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
