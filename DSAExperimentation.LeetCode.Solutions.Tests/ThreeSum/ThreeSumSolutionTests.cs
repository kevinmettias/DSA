using DSAExperimentation.LeetCode.Solutions.ThreeSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ThreeSum;

// Harness only. Both strategies are ThreeSumSolution's - this file pins them
// to four zeros, which offer several index triplets but only one distinct value
// triplet. Triplet order is not part of the contract for either strategy (the
// brute force collects into a HashSet), so cases are compared as sorted sets of
// triplets. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ThreeSumSolutionTests
{
    public static TheoryData<int[], (int First, int Second, int Third)[]> Cases =>
        new()
        {
            { [0, 0, 0, 0], [(0, 0, 0)] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindTripletsByBruteForce_Cases_ReturnsUniqueZeroSumTriplets(
        int[] nums, (int First, int Second, int Third)[] expected) =>
        AssertSameTriplets(expected, ThreeSumSolution.FindTripletsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindTripletsByMergeSortTwoPointers_Cases_ReturnsUniqueZeroSumTriplets(
        int[] nums, (int First, int Second, int Third)[] expected) =>
        AssertSameTriplets(expected, ThreeSumSolution.FindTripletsByMergeSortTwoPointers(nums));

    private static void AssertSameTriplets(
        (int First, int Second, int Third)[] expected,
        List<(int First, int Second, int Third)> actual) =>
        Assert.Equal(expected.OrderBy(triplet => triplet), actual.OrderBy(triplet => triplet));
}
