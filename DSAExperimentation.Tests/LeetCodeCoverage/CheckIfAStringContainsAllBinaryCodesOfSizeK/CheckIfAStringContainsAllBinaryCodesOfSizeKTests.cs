using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAStringContainsAllBinaryCodesOfSizeK;

// Harness only. Both the per-code substring search and the sliding-bitmask pass over
// this repo's own Set<int> are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's;
// this file pins them to LeetCode's published examples plus the boundary cases - a
// text exactly long enough to hold all 2^k windows, and one far too short to.
public sealed class CheckIfAStringContainsAllBinaryCodesOfSizeKTests
{
    public static TheoryData<string, int, bool> Examples =>
        new()
        {
            { "00110110", 2, true },
            { "0110", 1, true },
            { "0110", 2, false },
            { "111", 3, false },
            { "00110", 2, true },
            { "0000000", 3, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasAllCodesByCodeSubstringSearch_LeetCodeExamples_ReportsWhetherEveryCodeOccurs(
        string s, int k, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasAllCodesBySlidingBitmask_LeetCodeExamples_ReportsWhetherEveryCodeOccurs(
        string s, int k, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(s, k));
}
