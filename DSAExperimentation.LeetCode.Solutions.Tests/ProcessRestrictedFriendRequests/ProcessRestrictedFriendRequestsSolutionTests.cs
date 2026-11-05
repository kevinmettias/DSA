using DSAExperimentation.LeetCode.Solutions.ProcessRestrictedFriendRequests;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ProcessRestrictedFriendRequests;

// Harness only. Both strategies are ProcessRestrictedFriendRequestsSolution's -
// this file just pins them to a request whose
// rejection is purely transitive, a self-request, and a restriction-free run.
public sealed partial class ProcessRestrictedFriendRequestsSolutionTests
{
    public static TheoryData<int, int[][], int[][], bool[]> Cases =>
        new()
        {
            { 4, [[0, 3]], [[0, 1], [1, 2], [2, 3]], [true, true, false] },
            { 2, [[0, 1]], [[0, 0], [0, 1]], [true, false] },
            { 3, [], [[0, 1], [1, 2]], [true, true] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FriendRequestsByDisjointSet_Cases_ApprovesOnlyRequestsThatKeepRestrictedPairsApart(
        int personCount, int[][] restrictions, int[][] requests, bool[] expected)
    {
        var actual = ProcessRestrictedFriendRequestsSolution.FriendRequestsByDisjointSet(
            personCount, restrictions, requests);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FriendRequestsByReachabilityScan_Cases_ApprovesOnlyRequestsThatKeepRestrictedPairsApart(
        int personCount, int[][] restrictions, int[][] requests, bool[] expected)
    {
        var actual = ProcessRestrictedFriendRequestsSolution.FriendRequestsByReachabilityScan(
            personCount, restrictions, requests);

        Assert.Equal(expected, actual);
    }
}
