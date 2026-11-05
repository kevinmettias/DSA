using DSAExperimentation.LeetCode.Solutions.FindMinimumTimeToFinishAllJobs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindMinimumTimeToFinishAllJobs;

// Harness only. Both strategies - the exhaustive k^n assignment walk and the
// predicate search over feasible time limits - are FindMinimumTimeToFinishAllJobsSolution's,
// so this file only pins them to the cases that
// separate "minimum achievable maximum load" from the greedy answer.
public sealed partial class FindMinimumTimeToFinishAllJobsSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 2, 3, 4], 1, 10 },
            { [3, 5, 3, 4], 2, 8 },
            { [5], 2, 5 },
            { [1, 1, 1, 1], 4, 1 },
            { [9, 9, 9], 3, 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByExhaustiveAssignment_Cases_ReturnsSmallestAchievableMaximumLoad(
        int[] jobs, int workerCount, int expected)
    {
        var actual = FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByExhaustiveAssignment(jobs, workerCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByFeasibilityBinarySearch_Cases_ReturnsSmallestAchievableMaximumLoad(
        int[] jobs, int workerCount, int expected)
    {
        var actual = FindMinimumTimeToFinishAllJobsSolution.MinimumTimeByFeasibilityBinarySearch(jobs, workerCount);

        Assert.Equal(expected, actual);
    }
}
