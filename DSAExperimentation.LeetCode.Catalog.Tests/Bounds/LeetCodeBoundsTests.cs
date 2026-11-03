using DSAExperimentation.LeetCode.Catalog.Bounds;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Bounds;

public sealed partial class LeetCodeBoundsTests
{
    public static TheoryData<string, LeetCodeBound> Ranges =>
        new()
        {
            { "2 <= nums.length <= 10^4", new LeetCodeBound(2, true, ["nums.length"], 10_000, true) },
            { "-10^9 <= nums[i] <= 10^9", new LeetCodeBound(-1e9, true, ["nums[i]"], 1e9, true) },
            { "1 <= m, n <= 200", new LeetCodeBound(1, true, ["m", "n"], 200, true) },
            { "-2^31 <= val <= 2^31 - 1", new LeetCodeBound(-2147483648, true, ["val"], 2147483647, true) },
            { "0 < p < 1.", new LeetCodeBound(0, false, ["p"], 1, false) },
            { "1 <= words[i].length <= 2 * 10^5", new LeetCodeBound(1, true, ["words[i].length"], 200_000, true) },
        };

    public static TheoryData<string> NotRanges =>
        new()
        {
            "1 <= k <= nums.length",
            "0 <= start_i <= end_i <= 10^4",
            "m == grid.length",
            "nums[i] is either 0 or 1.",
            "The number of nodes in the tree is in the range [1, 10^4].",
        };

    [Theory]
    [MemberData(nameof(Ranges))]
    public void TryParse_TwoSidedNumericRange_ReadsLimitsOperatorsAndSubjects(string constraint, LeetCodeBound expected)
    {
        var isRange = LeetCodeBounds.TryParse(constraint, out var bound);

        Assert.True(isRange);
        Assert.NotNull(bound);
        Assert.Equal(expected.Lower, bound.Lower);
        Assert.Equal(expected.IsLowerInclusive, bound.IsLowerInclusive);
        Assert.Equal(expected.Subjects, bound.Subjects);
        Assert.Equal(expected.Upper, bound.Upper);
        Assert.Equal(expected.IsUpperInclusive, bound.IsUpperInclusive);
    }

    [Theory]
    [MemberData(nameof(NotRanges))]
    public void TryParse_RuleRatherThanRange_Declines(string constraint)
    {
        var isRange = LeetCodeBounds.TryParse(constraint, out _);

        Assert.False(isRange);
    }

    [Fact]
    public void Of_MixOfRangesAndRules_KeepsTheRangesInOrder()
    {
        var bounds = LeetCodeBounds.Of(["2 <= nums.length <= 10^4", "Only one valid answer exists.", "-10^9 <= target <= 10^9"]);

        Assert.Equal([["nums.length"], ["target"]], bounds.Select(bound => bound.Subjects));
    }
}
