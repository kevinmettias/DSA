using DSAExperimentation.LeetCode.LongestHappyPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestHappyPrefix;

// Harness only. Both strategies are LongestHappyPrefixSolution's - this file just
// pins them to LeetCode's published examples, one theory per strategy so a failure
// names the strategy that broke.
public sealed partial class LongestHappyPrefixTests
{
    public static TheoryData<HappyPrefixExample> Examples =>
        new()
        {
            // LeetCode example 1: "l" is the only prefix that is also a suffix.
            { new HappyPrefixExample(S: "level", Expected: "l") },

            // LeetCode example 2: the whole 4-character run repeats at the end.
            { new HappyPrefixExample(S: "leetcodeleet", Expected: "leet") },

            // Overlapping repeats: "abab" is both a prefix and a suffix of "ababab".
            { new HappyPrefixExample(S: "ababab", Expected: "abab") },

            // No prefix is also a suffix.
            { new HappyPrefixExample(S: "asdf", Expected: string.Empty) },

            // A one-character string has no PROPER prefix that is also a suffix.
            { new HappyPrefixExample(S: "a", Expected: string.Empty) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixByShrinkAndCompare_LeetCodeExamples_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        HappyPrefixExample example) =>
        Assert.Equal(example.Expected, LongestHappyPrefixSolution.LongestPrefixByShrinkAndCompare(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixByPrefixFunction_LeetCodeExamples_ReturnsLongestProperPrefixThatIsAlsoASuffix(
        HappyPrefixExample example) =>
        Assert.Equal(example.Expected, LongestHappyPrefixSolution.LongestPrefixByPrefixFunction(example.S));

    // One example as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct HappyPrefixExample(string S, string Expected);
}
