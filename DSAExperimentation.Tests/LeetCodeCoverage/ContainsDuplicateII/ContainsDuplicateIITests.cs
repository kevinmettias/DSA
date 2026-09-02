using DSAExperimentation.LeetCode.ContainsDuplicateII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicateII;

// Harness only: both strategies live in ContainsDuplicateIISolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed class ContainsDuplicateIITests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [1, 2, 3, 1], 3, true },
            { [1, 0, 1, 1], 1, true },
            { [1, 2, 3, 1, 2, 3], 2, false },
            { [1, 1], 1, true },
            { [1, 1], 0, false },
            { [1], 1, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyDuplicateByBruteForce_LeetCodeExamples_ReturnsExpected(
        int[] nums, int k, bool expected) =>
        Assert.Equal(expected, ContainsDuplicateIISolution.ContainsNearbyDuplicateByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsNearbyDuplicateByHashMap_LeetCodeExamples_ReturnsExpected(
        int[] nums, int k, bool expected) =>
        Assert.Equal(expected, ContainsDuplicateIISolution.ContainsNearbyDuplicateByHashMap(nums, k));
}
