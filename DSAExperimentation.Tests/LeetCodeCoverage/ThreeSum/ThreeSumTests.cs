using DSAExperimentation.LeetCode.ThreeSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSum;

// Harness only. Both strategies are ThreeSumSolution's - this file just pins
// them to LeetCode's published examples. Triplet order is not part of the
// contract for either strategy (the brute force collects into a HashSet), so
// examples are compared as sorted sets of triplets.
public sealed partial class ThreeSumTests
{
    public static TheoryData<int[], (int First, int Second, int Third)[]> Examples =>
        new()
        {
            { [-1, 0, 1, 2, -1, -4], [(-1, -1, 2), (-1, 0, 1)] },
            { [0, 1, 1], [] },
            { [0, 0, 0], [(0, 0, 0)] },
            { [0, 0, 0, 0], [(0, 0, 0)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTripletsByBruteForce_LeetCodeExamples_ReturnsUniqueZeroSumTriplets(
        int[] nums, (int First, int Second, int Third)[] expected) =>
        AssertSameTriplets(expected, ThreeSumSolution.FindTripletsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindTripletsByMergeSortTwoPointers_LeetCodeExamples_ReturnsUniqueZeroSumTriplets(
        int[] nums, (int First, int Second, int Third)[] expected) =>
        AssertSameTriplets(expected, ThreeSumSolution.FindTripletsByMergeSortTwoPointers(nums));

    private static void AssertSameTriplets(
        (int First, int Second, int Third)[] expected,
        List<(int First, int Second, int Third)> actual) =>
        Assert.Equal(expected.OrderBy(triplet => triplet), actual.OrderBy(triplet => triplet));
}
