using DSAExperimentation.LeetCode.ElevatorRequestsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ElevatorRequestsI;

// Harness only. Both strategies are ElevatorRequestsISolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class ElevatorRequestsITests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            { 5, [2, 1, 4, 3], 7 },
            { 3, [2, 0, 0], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalTimeByInlineAbsoluteDifference_LeetCodeExamples_ReturnsTotalSecondsToServeAllRequests(
        int floorCount, int[] requests, int expected)
    {
        var actual = ElevatorRequestsISolution.TotalTimeByInlineAbsoluteDifference(floorCount, requests);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalTimeByManhattanHeuristic_LeetCodeExamples_ReturnsTotalSecondsToServeAllRequests(
        int floorCount, int[] requests, int expected)
    {
        var actual = ElevatorRequestsISolution.TotalTimeByManhattanHeuristic(floorCount, requests);
        Assert.Equal(expected, actual);
    }
}
