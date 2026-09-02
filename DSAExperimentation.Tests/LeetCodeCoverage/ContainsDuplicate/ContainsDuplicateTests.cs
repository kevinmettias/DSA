using DSAExperimentation.LeetCode.ContainsDuplicate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicate;

// Harness only: both strategies live in ContainsDuplicateSolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class ContainsDuplicateTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 1], true },
            { [1, 2, 3, 4], false },
            { [1, 1, 1, 3, 3, 4, 3, 2, 4, 2], true },
            { [], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsDuplicateByBruteForce_LeetCodeExamples_ReturnsExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, ContainsDuplicateSolution.ContainsDuplicateByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsDuplicateBySetProbe_LeetCodeExamples_ReturnsExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, ContainsDuplicateSolution.ContainsDuplicateBySetProbe(nums));
}
