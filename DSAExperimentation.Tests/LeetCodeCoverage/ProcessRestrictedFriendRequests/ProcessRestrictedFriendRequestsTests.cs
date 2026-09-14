using DSAExperimentation.LeetCode.ProcessRestrictedFriendRequests;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProcessRestrictedFriendRequests;

// Harness only. Both strategies are ProcessRestrictedFriendRequestsSolution's -
// this file just pins them to LeetCode's published examples, plus a request whose
// rejection is purely transitive, a self-request, and a restriction-free run.
public sealed class ProcessRestrictedFriendRequestsTests
{
    public static TheoryData<int, int[][], int[][], bool[]> Examples =>
        new()
        {
            { 3, [[0, 1]], [[0, 2], [2, 1]], [true, false] },
            { 3, [[0, 1]], [[1, 2], [0, 2]], [true, false] },
            { 5, [[0, 1], [1, 2], [2, 3]], [[0, 4], [1, 2], [3, 1], [3, 4]], [true, false, true, false] },
            { 4, [[0, 3]], [[0, 1], [1, 2], [2, 3]], [true, true, false] },
            { 2, [[0, 1]], [[0, 0], [0, 1]], [true, false] },
            { 3, [], [[0, 1], [1, 2]], [true, true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FriendRequestsByDisjointSet_LeetCodeExamples_ApprovesOnlyRequestsThatKeepRestrictedPairsApart(
        int n, int[][] restrictions, int[][] requests, bool[] expected) =>
        Assert.Equal(
            expected,
            ProcessRestrictedFriendRequestsSolution.FriendRequestsByDisjointSet(n, restrictions, requests));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FriendRequestsByReachabilityScan_LeetCodeExamples_ApprovesOnlyRequestsThatKeepRestrictedPairsApart(
        int n, int[][] restrictions, int[][] requests, bool[] expected) =>
        Assert.Equal(
            expected,
            ProcessRestrictedFriendRequestsSolution.FriendRequestsByReachabilityScan(n, restrictions, requests));
}
