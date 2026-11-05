using DSAExperimentation.LeetCode.Solutions.FindServersThatHandledMostNumberOfRequests;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindServersThatHandledMostNumberOfRequests;

public sealed partial class FindServersThatHandledMostNumberOfRequestsPromiseTests
{
    public static TheoryData<int, int[], int[], bool> Inputs =>
        new()
        {
            { 3, [1, 2, 3, 4, 5], [5, 2, 3, 3, 3], true },
            { 3, [1, 2, 3], [5, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryRequestHasOneLoad(int serverCount, int[] arrival, int[] load, bool expected)
        => Assert.Equal(expected, FindServersThatHandledMostNumberOfRequestsPromise.IsKeptBy(serverCount, arrival, load));
}
