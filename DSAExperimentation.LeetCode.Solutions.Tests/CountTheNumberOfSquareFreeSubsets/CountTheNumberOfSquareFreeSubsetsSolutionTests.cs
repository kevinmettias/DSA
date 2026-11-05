using DSAExperimentation.LeetCode.Solutions.CountTheNumberOfSquareFreeSubsets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountTheNumberOfSquareFreeSubsets;

// Harness only: the algorithms live in CountTheNumberOfSquareFreeSubsetsSolution. One
// test method per strategy over one shared set of cases, so a failure names the
// strategy that broke (TwoSumTests precedent). The cases cover repeated 1s and
// values that each carry a squared factor on their own. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class CountTheNumberOfSquareFreeSubsetsSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [1, 1], 3 }, // {1a}, {1b}, {1a,1b} - 1 never breaks square-freeness
            { [4, 8, 16], 0 }, // every value already has a squared factor of 2 on its own
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBruteForce_Cases_ReturnsSquareFreeSubsetCount(int[] nums, long expected)
    {
        var actual = CountTheNumberOfSquareFreeSubsetsSolution.CountByBruteForce(nums);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBitmaskMemo_Cases_ReturnsSquareFreeSubsetCount(int[] nums, long expected)
    {
        var actual = CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo(nums);
        Assert.Equal(expected, actual);
    }
}
