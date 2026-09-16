using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAStringContainsAllBinaryCodesOfSizeK;

// Harness only. Both the per-code substring search and the sliding-bitmask pass over
// this repo's own Set<int> are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's;
// this file pins them to LeetCode's published examples plus the boundary cases - a
// text exactly long enough to hold all 2^k windows, and one far too short to.
public sealed partial class CheckIfAStringContainsAllBinaryCodesOfSizeKTests
{
    public static TheoryData<AllCodesExample> Examples =>
        new()
        {
            { new AllCodesExample(S: "00110110", K: 2, Expected: true) },
            { new AllCodesExample(S: "0110", K: 1, Expected: true) },
            { new AllCodesExample(S: "0110", K: 2, Expected: false) },
            { new AllCodesExample(S: "111", K: 3, Expected: false) },
            { new AllCodesExample(S: "00110", K: 2, Expected: true) },
            { new AllCodesExample(S: "0000000", K: 3, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasAllCodesByCodeSubstringSearch_LeetCodeExamples_ReportsWhetherEveryCodeOccurs(AllCodesExample example)
    {
        var allCodesPresent =
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(example.S, example.K);

        Assert.Equal(example.Expected, allCodesPresent);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasAllCodesBySlidingBitmask_LeetCodeExamples_ReportsWhetherEveryCodeOccurs(AllCodesExample example)
    {
        var allCodesPresent =
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(example.S, example.K);

        Assert.Equal(example.Expected, allCodesPresent);
    }

    // One LeetCode example: the binary text, the code length, and whether every
    // length-k code occurs in it. The row names every position - a bare `bool` argument
    // would read as "true" and say nothing about what is true.
    public readonly record struct AllCodesExample(string S, int K, bool Expected);
}
