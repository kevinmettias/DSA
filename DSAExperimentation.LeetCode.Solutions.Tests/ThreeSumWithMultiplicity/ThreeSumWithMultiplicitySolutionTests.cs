using DSAExperimentation.LeetCode.Solutions.ThreeSumWithMultiplicity;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ThreeSumWithMultiplicity;

// Harness only: both counting strategies live in ThreeSumWithMultiplicitySolution.
// One test method per strategy over one shared set of cases, so a failure names
// the strategy that broke - which is what finally puts the cubic baseline under
// test, since it previously existed only as a benchmark arm nothing asserted. The
// cases are a target no triplet reaches and an array of equal values. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class ThreeSumWithMultiplicitySolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [0, 0, 0], 1, 0 }, // no triplet reaches the target
            { [0, 0, 0, 0], 0, 4 }, // C(4, 3) = 4, every triplet of equal values
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountTripletsByBruteForce_Cases_ReturnsMatchingIndexTripletCount(
        int[] arr, int target, int expected)
    {
        var actual = ThreeSumWithMultiplicitySolution.CountTripletsByBruteForce(arr, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountTripletsByMergeSortTwoPointers_Cases_ReturnsMatchingIndexTripletCount(
        int[] arr, int target, int expected)
    {
        var actual = ThreeSumWithMultiplicitySolution.CountTripletsByMergeSortTwoPointers(arr, target);

        Assert.Equal(expected, actual);
    }
}
