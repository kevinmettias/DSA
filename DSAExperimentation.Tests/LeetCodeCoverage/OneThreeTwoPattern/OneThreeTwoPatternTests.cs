using DSAExperimentation.LeetCode.OneThreeTwoPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OneThreeTwoPattern;

// Harness only. Both strategies are OneThreeTwoPatternSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class OneThreeTwoPatternTests
{
    public static TheoryData<PatternExample> Examples =>
        new()
        {
            { new PatternExample(Nums: [1, 2, 3, 4], Expected: false) },
            { new PatternExample(Nums: [3, 1, 4, 2], Expected: true) },
            { new PatternExample(Nums: [-1, 3, 2, 0], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPatternByBruteForce_LeetCodeExamples_MatchesExpected(PatternExample example) =>
        Assert.Equal(example.Expected, OneThreeTwoPatternSolution.HasPatternByBruteForce(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPatternByMonotonicStack_LeetCodeExamples_MatchesExpected(PatternExample example) =>
        Assert.Equal(example.Expected, OneThreeTwoPatternSolution.HasPatternByMonotonicStack(example.Nums));

    // One LeetCode example: the sequence and whether it holds an i < j < k with
    // nums[i] < nums[k] < nums[j]. The outcome is the datum under test, so the row
    // names it rather than leaving a bare `bool` beside the array.
    public readonly record struct PatternExample(int[] Nums, bool Expected);
}
