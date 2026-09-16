using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfDaysToEatNOrangesBenchmarks (ARCHITECTURE 17.9): both arms
// are MinimumNumberOfDaysToEatNOrangesSolution's, the same methods
// MinimumNumberOfDaysToEatNOrangesTests proves correct, and both walk the same recurrence -
// only whether repeated states are cached differs - so arms that disagree are timing two
// different problems.
//
// There is no workload to rebuild here: the input is the [Params] value itself, so the harness
// is a bare initializer and the two arms are compared directly across one shared harness.
public sealed partial class MinimumNumberOfDaysToEatNOrangesBenchmarksTests
{
    // The smallest declared [Params] value: the unmemoized arm's call tree is the thing under
    // comparison, and its size is what the larger case is there to exaggerate.
    private const int SmallestOrangeCount = 100_000;

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecurrence_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecurrence());
    }

    private static MinimumNumberOfDaysToEatNOrangesBenchmarks BuildHarness() =>
        new() { OrangeCount = SmallestOrangeCount };
}
