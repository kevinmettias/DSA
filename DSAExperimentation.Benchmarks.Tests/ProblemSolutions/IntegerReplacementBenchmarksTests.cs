using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IntegerReplacementBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - the bare recurrence against a memoized one over the same
// StartValue - so a harness whose arms disagree is timing two different problems. Both arms answer
// with the single minimum step count, an int, compared directly. This class has no [GlobalSetup]:
// StartValue is the whole input, so each [Fact] constructs the harness with the smaller of the two
// [Params] values and calls both arms. Nothing here is random, so the same StartValue must return
// the same step count.
public sealed partial class IntegerReplacementBenchmarksTests
{
    private const int SmallestStartValue = 21_845;

    [Fact]
    public void Setup_SameStartValue_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MemoizedRecurrence(), BuildHarness().MemoizedRecurrence());

    [Fact]
    public void UnmemoizedRecursion_MinimumSteps_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_MinimumSteps_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecurrence());
    }

    private static IntegerReplacementBenchmarks BuildHarness() =>
        new() { StartValue = SmallestStartValue };
}
