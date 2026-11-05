using DSAExperimentation.LeetCode.Solutions.FindBuildingWhereAliceAndBobCanMeet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindBuildingWhereAliceAndBobCanMeet;

// Harness only. Both strategies live in
// FindBuildingWhereAliceAndBobCanMeetSolution - this file pins them to worked
// examples that exercise every branch of LC 2940's own reachability rule: a
// trivial same-building query, a one-hop direct jump, a forward search that finds
// its answer immediately after hi, one that has to look further, a forward search
// with no answer at all, and a query given with its two indices in the opposite
// (b, a) order to confirm the answer only depends on { a, b } as a set. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class FindBuildingWhereAliceAndBobCanMeetSolutionTests
{
    public static TheoryData<int[], int[][], int[]> Cases =>
        new()
        {
            // Worked examples.
            {
                [4, 2, 3, 8, 1, 6],
                [[1, 2], [3, 3], [0, 2], [2, 4], [3, 4], [2, 0]],
                [2, 3, 3, 5, -1, 3]
            },
            {
                [3, 2, 5, 4, 1, 2, 7],
                [[3, 5], [1, 2], [0, 4], [2, 3], [5, 6], [4, 4]],
                [6, 2, 6, 6, 6, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMeetingBuildingsByBruteForce_WorkedExamples_ReturnsEarliestMeetingBuildings(
        int[] heights, int[][] queries, int[] expected)
    {
        var actual = FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByBruteForce(heights, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMeetingBuildingsByOfflineHeapSweep_WorkedExamples_ReturnsEarliestMeetingBuildings(
        int[] heights, int[][] queries, int[] expected)
    {
        var actual = FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByOfflineHeapSweep(heights, queries);

        Assert.Equal(expected, actual);
    }
}
