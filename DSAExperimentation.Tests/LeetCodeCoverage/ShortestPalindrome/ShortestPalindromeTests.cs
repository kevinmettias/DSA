using DSAExperimentation.LeetCode.ShortestPalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPalindrome;

// Harness only. Both strategies are ShortestPalindromeSolution's - this file just
// pins them to LeetCode's published examples plus the edge cases the original
// coverage carried (empty string, single char, already-palindrome).
public sealed class ShortestPalindromeTests
{
    public static TheoryData<PalindromeExample> Examples =>
        new()
        {
            new PalindromeExample(Input: "aacecaaa", Expected: "aaacecaaa"),
            new PalindromeExample(Input: "abcd", Expected: "dcbabcd"),
            new PalindromeExample(Input: "", Expected: ""),
            new PalindromeExample(Input: "a", Expected: "a"),
            new PalindromeExample(Input: "aa", Expected: "aa"),
            new PalindromeExample(Input: "racecar", Expected: "racecar"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildShortestPalindromeByNaiveScan_LeetCodeExamples_PrependsFewestCharacters(
        PalindromeExample example) =>
        Assert.Equal(example.Expected, ShortestPalindromeSolution.BuildShortestPalindromeByNaiveScan(example.Input));

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildShortestPalindromeByKmpFailureFunction_LeetCodeExamples_PrependsFewestCharacters(
        PalindromeExample example) =>
        Assert.Equal(
            example.Expected,
            ShortestPalindromeSolution.BuildShortestPalindromeByKmpFailureFunction(example.Input));

    // One LeetCode example: the string and the shortest palindrome it builds. The
    // two are both strings in adjacent positions at the call site, so the bundle
    // names the input and the expectation rather than leaving them swappable.
    public readonly record struct PalindromeExample(string Input, string Expected);
}
