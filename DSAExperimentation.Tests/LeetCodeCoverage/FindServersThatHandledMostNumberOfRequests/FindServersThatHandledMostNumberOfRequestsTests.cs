using DSAExperimentation.LeetCode.FindServersThatHandledMostNumberOfRequests;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindServersThatHandledMostNumberOfRequests;

// Harness only. Both strategies are
// FindServersThatHandledMostNumberOfRequestsSolution's - this file pins them to
// LeetCode's published examples plus the two shapes the ring search has to get right:
// a request that finds every server busy and is dropped, and a request whose
// preferred server index is past the only free one, so the search has to wrap.
public sealed class FindServersThatHandledMostNumberOfRequestsTests
{
    public static TheoryData<int, int[], int[], int[]> Examples =>
        new()
        {
            { 3, [1, 2, 3, 4, 5], [5, 2, 3, 3, 3], [1] },
            { 3, [1, 2, 3, 4], [1, 2, 1, 2], [0] },
            { 3, [1, 2, 3], [10, 12, 11], [0, 1, 2] },
            { 1, [1], [1], [0] },
            // Request 4 prefers server 1, but only server 0 is free - the search has
            // to wrap past the end of the ring to find it.
            { 3, [1, 2, 3, 4, 5], [1, 5, 5, 1, 1], [0] },
            // Requests 3 and 4 arrive while all three servers are still busy and are
            // dropped, leaving every server tied on one request.
            { 3, [1, 2, 3, 4, 5], [5, 5, 5, 5, 5], [0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BusiestServersByLinearScanRing_LeetCodeExamples_ReturnsBusiestServers(
        int k, int[] arrival, int[] load, int[] expected) =>
        Assert.Equal(
            expected,
            FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByLinearScanRing(k, arrival, load));

    [Theory]
    [MemberData(nameof(Examples))]
    public void BusiestServersByFenwickCeilingAndHeap_LeetCodeExamples_ReturnsBusiestServers(
        int k, int[] arrival, int[] load, int[] expected) =>
        Assert.Equal(
            expected,
            FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByFenwickCeilingAndHeap(k, arrival, load));
}
