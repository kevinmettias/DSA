using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfOperationsToMakeXAndYEqualBenchmarks (ARCHITECTURE 17.9):
// both arms are MinimumNumberOfOperationsToMakeXAndYEqualSolution's, the same methods
// MinimumNumberOfOperationsToMakeXAndYEqualTests proves correct, and both return the fewest
// operations that reduce the start value to the target. The mutation-queue BFS and the memoized
// recurrence are competing strategies for that one number, so arms that disagree are timing two
// different problems.
//
// There is no workload to rebuild here: the start value is the [Params] value itself and the
// target is the class's own constant, so the harness is a bare initializer and the two arms are
// compared directly across one shared harness.
public sealed partial class MinimumNumberOfOperationsToMakeXAndYEqualBenchmarksTests
{
    // The smallest declared [Params] value: it is deliberately far from a multiple of either
    // divisor, so the queue arm already has to explore the bounded range at this size.
    private const int SmallestStartValue = 997;

    [Fact]
    public void MutationQueueBfs_AgreesWithMemoizedReduce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedReduce(), harness.MutationQueueBfs());
    }

    [Fact]
    public void MemoizedReduce_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.MemoizedReduce());
    }

    private static MinimumNumberOfOperationsToMakeXAndYEqualBenchmarks BuildHarness() =>
        new() { StartValue = SmallestStartValue };
}
