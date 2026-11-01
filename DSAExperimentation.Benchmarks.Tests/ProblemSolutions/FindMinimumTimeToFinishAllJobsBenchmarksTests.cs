using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumTimeToFinishAllJobsBenchmarks (ARCHITECTURE 17.9): the generic arms
// check only compares the arms with each other, so this bounds the baseline's answer by the workload
// itself. Setup's jobs are seeded positives, so the makespan is at least the longest single job and
// at most the sum of them all - in particular, positive.
public sealed partial class FindMinimumTimeToFinishAllJobsBenchmarksTests
{
    // The smaller of Setup's [Params(8, 10)] job counts, the one whose exhaustive arm is affordable.
    private const int SmallestJobCount = 8;

    [Fact]
    public void ExhaustiveAssignment_SeededPositiveJobs_ReturnsAPositiveMakespan() =>
        Assert.True(BuildHarness().ExhaustiveAssignment() > 0);

    private static FindMinimumTimeToFinishAllJobsBenchmarks BuildHarness()
    {
        var harness = new FindMinimumTimeToFinishAllJobsBenchmarks { JobCount = SmallestJobCount };
        harness.Setup();

        return harness;
    }
}
