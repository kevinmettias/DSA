using DSAExperimentation.LeetCode.FindBuildingWhereAliceAndBobCanMeet;

namespace DSAExperimentation.LeetCode.Tests.FindBuildingWhereAliceAndBobCanMeet;

// Harness only. Both strategies live in
// FindBuildingWhereAliceAndBobCanMeetSolution - this file pins them to
// LeetCode's two published examples, then to worked examples that exercise
// every branch of LC 2940's own reachability rule: a trivial same-building
// query, a one-hop direct jump, a forward search that finds its answer
// immediately after hi, one that has to look further, a forward search with no
// answer at all, and a query given with its two indices in the opposite (b, a)
// order to confirm the answer only depends on { a, b } as a set.
public sealed partial class FindBuildingWhereAliceAndBobCanMeetSolutionTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            // LeetCode example 1.
            {
                [6, 4, 8, 5, 2, 7],
                [[0, 1], [0, 3], [2, 4], [3, 4], [2, 2]],
                [2, 5, -1, 5, 2]
            },

            // LeetCode example 2.
            {
                [5, 3, 8, 2, 6, 1, 4, 6],
                [[0, 7], [3, 5], [5, 2], [3, 0], [1, 6]],
                [7, 6, -1, 4, 6]
            },

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
    [MemberData(nameof(Examples))]
    public void FindMeetingBuildingsByBruteForce_WorkedExamples_ReturnsEarliestMeetingBuildings(
        int[] heights, int[][] queries, int[] expected)
    {
        var actual = FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByBruteForce(heights, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMeetingBuildingsByOfflineHeapSweep_WorkedExamples_ReturnsEarliestMeetingBuildings(
        int[] heights, int[][] queries, int[] expected)
    {
        var actual = FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByOfflineHeapSweep(heights, queries);

        Assert.Equal(expected, actual);
    }
}
