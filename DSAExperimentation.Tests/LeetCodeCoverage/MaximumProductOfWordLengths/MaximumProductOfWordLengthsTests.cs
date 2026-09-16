using DSAExperimentation.LeetCode.MaximumProductOfWordLengths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfWordLengths;

// Harness only: both arms are MaximumProductOfWordLengthsSolution's, the same
// methods MaximumProductOfWordLengthsBenchmarks measures.
public sealed partial class MaximumProductOfWordLengthsTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["abcw", "baz", "foo", "bar", "xtfn", "abcdef"], 16 },
            { ["a", "ab", "abc", "d", "cd", "bcd", "abcd"], 4 },
            { ["a", "aa", "aaa", "aaaa"], 0 },
            { ["a", "b"], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByCharacterScan_LeetCodeExamples_ReturnsLargestDisjointProduct(
        string[] words, int expected) =>
        Assert.Equal(expected, MaximumProductOfWordLengthsSolution.MaxProductByCharacterScan(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByBitmaskHashMap_LeetCodeExamples_ReturnsLargestDisjointProduct(
        string[] words, int expected) =>
        Assert.Equal(expected, MaximumProductOfWordLengthsSolution.MaxProductByBitmaskHashMap(words));
}
