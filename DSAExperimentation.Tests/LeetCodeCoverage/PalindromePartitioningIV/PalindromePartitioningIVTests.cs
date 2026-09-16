using DSAExperimentation.LeetCode.PalindromePartitioningIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningIV;

// Harness only: both strategies live in PalindromePartitioningIVSolution and are
// asserted against the same examples, so a failure names the strategy that broke.
// The unmemoized recursion was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time.
public sealed class PalindromePartitioningIVTests
{
    public static TheoryData<PartitionExample> Examples =>
        new()
        {
            { new PartitionExample(Text: "abcbdd", Expected: true) },
            { new PartitionExample(Text: "bcbddxy", Expected: false) },
            { new PartitionExample(Text: "abc", Expected: true) },
            { new PartitionExample(Text: "aba", Expected: true) },
            { new PartitionExample(Text: "aaaa", Expected: true) },
            { new PartitionExample(Text: "abcde", Expected: false) },
            { new PartitionExample(Text: "ab", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckPartitioningByNaiveRecursion_LeetCodeExamples_ReturnsWhetherThreeWaySplitExists(
        PartitionExample example) =>
        Assert.Equal(
            example.Expected, PalindromePartitioningIVSolution.CheckPartitioningByNaiveRecursion(example.Text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckPartitioningByMemoizedRecurrence_LeetCodeExamples_ReturnsWhetherThreeWaySplitExists(
        PartitionExample example) =>
        Assert.Equal(
            example.Expected, PalindromePartitioningIVSolution.CheckPartitioningByMemoizedRecurrence(example.Text));

    // One LeetCode example: the string to cut into three non-empty palindromes, and
    // whether such a cut exists. The answer is the datum under test, so the row names
    // it rather than leaving a bare `bool` beside the string.
    public readonly record struct PartitionExample(string Text, bool Expected);
}
