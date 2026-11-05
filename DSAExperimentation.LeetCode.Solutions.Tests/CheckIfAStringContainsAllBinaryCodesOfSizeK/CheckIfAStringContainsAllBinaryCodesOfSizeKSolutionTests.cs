using DSAExperimentation.LeetCode.Solutions.CheckIfAStringContainsAllBinaryCodesOfSizeK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfAStringContainsAllBinaryCodesOfSizeK;

// Harness only. Both the per-code substring search and the sliding-bitmask pass over
// this repo's own Set<int> are CheckIfAStringContainsAllBinaryCodesOfSizeKSolution's;
// this file pins them to the boundary cases - a
// text exactly long enough to hold all 2^k windows, and one far too short to.
public sealed partial class CheckIfAStringContainsAllBinaryCodesOfSizeKSolutionTests
{
    public static TheoryData<AllCodesCase> Cases =>
        new()
        {
            { new AllCodesCase(S: "111", K: 3, Expected: false) },
            { new AllCodesCase(S: "00110", K: 2, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasAllCodesByCodeSubstringSearch_Cases_ReportsWhetherEveryCodeOccurs(AllCodesCase testCase)
    {
        var allCodesPresent =
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(testCase.S, testCase.K);

        Assert.Equal(testCase.Expected, allCodesPresent);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasAllCodesBySlidingBitmask_Cases_ReportsWhetherEveryCodeOccurs(AllCodesCase testCase)
    {
        var allCodesPresent =
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(testCase.S, testCase.K);

        Assert.Equal(testCase.Expected, allCodesPresent);
    }

    // One case: the binary text, the code length, and whether every
    // length-k code occurs in it. The row names every position - a bare `bool` argument
    // would read as "true" and say nothing about what is true.
    public readonly record struct AllCodesCase(string S, int K, bool Expected);
}
