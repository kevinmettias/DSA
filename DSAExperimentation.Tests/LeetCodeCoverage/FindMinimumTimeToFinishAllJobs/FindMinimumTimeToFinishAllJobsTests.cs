using DSAExperimentation.LeetCode.FindMinimumTimeToFinishAllJobs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumTimeToFinishAllJobs;

// Harness only. Both strategies - the exhaustive k^n assignment walk and the
// LowerBound-over-feasibility search - are FindMinimumTimeToFinishAllJobsSolution's,
// so this file only pins them to LeetCode's published examples plus the cases that
// separate "minimum achievable maximum load" from the greedy answer.
public sealed class FindMinimumTimeToFinishAllJobsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 2, 3], 3, 3 },
            { [1, 2, 4, 7, 8], 2, 11 },
            { [1, 2, 3, 4], 1, 10 },
            { [3, 5, 3, 4], 2, 8 },
            { [5], 2, 5 },
            { [1, 1, 1, 1], 4, 1 },
            { [9, 9, 9], 3, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByExhaustiveAssignment_LeetCodeExamples_ReturnsSmallestAchievableMaximumLoad(
        int[] jobs, int workerCount, int expected)
    {
        var actual = FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByExhaustiveAssignment(jobs, workerCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByFeasibilityBinarySearch_LeetCodeExamples_ReturnsSmallestAchievableMaximumLoad(
        int[] jobs, int workerCount, int expected)
    {
        var actual = FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByFeasibilityBinarySearch(jobs, workerCount);

        Assert.Equal(expected, actual);
    }
}
