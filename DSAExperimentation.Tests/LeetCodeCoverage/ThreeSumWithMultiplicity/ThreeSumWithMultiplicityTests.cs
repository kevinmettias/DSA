using DSAExperimentation.LeetCode.ThreeSumWithMultiplicity;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSumWithMultiplicity;

// Harness only: both counting strategies live in ThreeSumWithMultiplicitySolution.
// One test method per strategy over one shared set of examples, so a failure names
// the strategy that broke - which is what finally puts the cubic baseline under
// test, since it previously existed only as a benchmark arm nothing asserted.
public sealed class ThreeSumWithMultiplicityTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 1, 2, 2, 3, 3, 4, 4, 5, 5], 8, 20 }, // LC example 1
            { [1, 1, 2, 2, 2, 2], 5, 12 }, // LC example 2: 2 * C(4, 2) = 12
            { [2, 1, 3], 6, 1 }, // LC example 3: the one triplet, in some order
            { [0, 0, 0], 1, 0 }, // no triplet reaches the target
            { [0, 0, 0], 0, 1 }, // the single index triplet i < j < k
            { [0, 0, 0, 0], 0, 4 }, // C(4, 3) = 4, every triplet of equal values
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTripletsByBruteForce_LeetCodeExamples_ReturnsMatchingIndexTripletCount(
        int[] arr, int target, int expected)
    {
        var actual = ThreeSumWithMultiplicitySolution.CountTripletsByBruteForce(arr, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTripletsByMergeSortTwoPointers_LeetCodeExamples_ReturnsMatchingIndexTripletCount(
        int[] arr, int target, int expected)
    {
        var actual = ThreeSumWithMultiplicitySolution.CountTripletsByMergeSortTwoPointers(arr, target);

        Assert.Equal(expected, actual);
    }
}
