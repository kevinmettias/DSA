using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumTimeToFinishAllJobsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating every one of the WorkerCount^JobCount
// assignments against binary-searching the answer with a pruned feasibility check at each candidate
// limit - so a harness whose arms disagree has optimized two different objective functions. Both
// answers are one int, so they are compared directly.
//
// Setup's jobs are the same seeded positives for a given JobCount, so the same parameters must
// rebuild the same instance and therefore the same optimal makespan. With every job positive the
// makespan is at least the longest single job and at most the sum of them all.
public sealed partial class FindMinimumTimeToFinishAllJobsBenchmarksTests
{
    // The smaller of Setup's [Params(8, 10)] job counts, the one whose exhaustive arm is affordable.
    private const int SmallestJobCount = 8;

    [Fact]
    public void Setup_SameJobCount_RebuildsTheSameOptimalMakespan() =>
        Assert.Equal(BuildHarness().ExhaustiveAssignment(), BuildHarness().ExhaustiveAssignment());

    [Fact]
    public void ExhaustiveAssignment_SeededPositiveJobs_AgreesWithBinarySearchWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.True(harness.ExhaustiveAssignment() > 0);
        Assert.Equal(harness.BinarySearchWithBacktracking(), harness.ExhaustiveAssignment());
    }

    [Fact]
    public void BinarySearchWithBacktracking_SeededPositiveJobs_AgreesWithExhaustiveAssignment()
    {
        var harness = BuildHarness();

        Assert.True(harness.BinarySearchWithBacktracking() > 0);
        Assert.Equal(harness.ExhaustiveAssignment(), harness.BinarySearchWithBacktracking());
    }

    private static FindMinimumTimeToFinishAllJobsBenchmarks BuildHarness()
    {
        var harness = new FindMinimumTimeToFinishAllJobsBenchmarks { JobCount = SmallestJobCount };
        harness.Setup();

        return harness;
    }
}
