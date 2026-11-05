using DSAExperimentation.LeetCode.Solutions.PalindromePartitioningIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PalindromePartitioningIV;

// Harness only: both strategies live in PalindromePartitioningIVSolution and are
// asserted against the same cases, so a failure names the strategy that broke.
// The unmemoized recursion was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time.
public sealed partial class PalindromePartitioningIVSolutionTests
{
    public static TheoryData<PartitionCase> Cases =>
        new()
        {
            { new PartitionCase(Text: "aba", Expected: true) },
            { new PartitionCase(Text: "aaaa", Expected: true) },
            { new PartitionCase(Text: "abcde", Expected: false) },
            { new PartitionCase(Text: "ab", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanPartitionIntoThreePalindromesByNaiveRecursion_Cases_ReturnsWhetherThreeWaySplitExists(
        PartitionCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            PalindromePartitioningIVSolution.CanPartitionIntoThreePalindromesByNaiveRecursion(testCase.Text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanPartitionIntoThreePalindromesByMemoizedRecurrence_Cases_ReturnsWhetherThreeWaySplitExists(
        PartitionCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            PalindromePartitioningIVSolution.CanPartitionIntoThreePalindromesByMemoizedRecurrence(testCase.Text));

    // One case: the string to cut into three non-empty palindromes, and
    // whether such a cut exists. The answer is the datum under test, so the row names
    // it rather than leaving a bare `bool` beside the string.
    public readonly record struct PartitionCase(string Text, bool Expected);
}
