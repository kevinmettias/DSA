using DSAExperimentation.LeetCode.Solutions.FindServersThatHandledMostNumberOfRequests;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindServersThatHandledMostNumberOfRequests;

// Harness only. Both strategies are
// FindServersThatHandledMostNumberOfRequestsSolution's - this file pins them to
// the two shapes the ring search has to get right:
// a request that finds every server busy and is dropped, and a request whose
// preferred server index is past the only free one, so the search has to wrap.
public sealed partial class FindServersThatHandledMostNumberOfRequestsSolutionTests
{
    public static TheoryData<int, int[], int[], int[]> Cases =>
        new()
        {
            { 1, [1], [1], [0] },
            // Request 4 prefers server 1, but only server 0 is free - the search has
            // to wrap past the end of the ring to find it.
            { 3, [1, 2, 3, 4, 5], [1, 5, 5, 1, 1], [0] },
            // Requests 3 and 4 arrive while all three servers are still busy and are
            // dropped, leaving every server tied on one request.
            { 3, [1, 2, 3, 4, 5], [5, 5, 5, 5, 5], [0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void BusiestServersByLinearScanRing_Cases_ReturnsBusiestServers(
        int serverCount, int[] arrival, int[] load, int[] expected)
    {
        var actual = FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByLinearScanRing(
            serverCount, arrival, load);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void BusiestServersByFenwickCeilingAndHeap_Cases_ReturnsBusiestServers(
        int serverCount, int[] arrival, int[] load, int[] expected)
    {
        var actual = FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByFenwickCeilingAndHeap(
            serverCount, arrival, load);

        Assert.Equal(expected, actual);
    }
}
