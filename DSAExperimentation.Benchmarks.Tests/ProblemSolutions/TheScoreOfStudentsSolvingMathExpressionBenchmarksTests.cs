using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheScoreOfStudentsSolvingMathExpressionBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the plain interval recursion that
// recomputes every (left, right) sub-interval from scratch against the identical recurrence over
// this repo's Memoizer<TState,TResult> - so a harness whose arms disagree is timing two different
// problems. Both arms return the students' score as an int, so they are compared directly. Setup
// builds the expression and the answer list from one fixed seed, so the same NumberCount must
// rebuild the same workload.
public sealed partial class TheScoreOfStudentsSolvingMathExpressionBenchmarksTests
{
    private const int SmallestNumberCount = 6;

    [Fact]
    public void Setup_SameNumberCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_SmallestNumberCount_AgreesWithMemoizedIntervals()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedIntervals(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedIntervals_SmallestNumberCount_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedIntervals());
    }

    private static TheScoreOfStudentsSolvingMathExpressionBenchmarks BuildHarness()
    {
        var harness = new TheScoreOfStudentsSolvingMathExpressionBenchmarks
        {
            NumberCount = SmallestNumberCount,
        };
        harness.Setup();

        return harness;
    }
}
