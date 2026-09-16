using DSAExperimentation.LeetCode.NumberOfStringsWhichCanBeRearrangedToContainSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfStringsWhichCanBeRearrangedToContainSubstring;

// Harness only: both strategies live in
// NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names
// the strategy that broke (TwoSumTests precedent).
public sealed class NumberOfStringsWhichCanBeRearrangedToContainSubstringTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 4, 12 },
            { 10, 83_943_898 },
            { 100, 86_731_066 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRearrangeableStringsByStateDp_LeetCodeExamples_ReturnsCountModulo1e9Plus7(
        int stringLength, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution.CountRearrangeableStringsByStateDp(stringLength));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRearrangeableStringsByInclusionExclusion_LeetCodeExamples_ReturnsCountModulo1e9Plus7(
        int stringLength, int expected) =>
        Assert.Equal(
            expected,
            NumberOfStringsWhichCanBeRearrangedToContainSubstringSolution.CountRearrangeableStringsByInclusionExclusion(stringLength));
}
