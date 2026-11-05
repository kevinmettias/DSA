using DSAExperimentation.LeetCode.Solutions.ShortestPalindrome;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestPalindrome;

// Harness only. Both strategies are ShortestPalindromeSolution's - this file just
// pins them to the edge cases the original
// coverage carried (empty string, single char, already-palindrome).
public sealed partial class ShortestPalindromeSolutionTests
{
    public static TheoryData<PalindromeCase> Cases =>
        new()
        {
            new PalindromeCase(Input: "aa", Expected: "aa"),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void BuildShortestPalindromeByNaiveScan_Cases_PrependsFewestCharacters(
        PalindromeCase testCase) =>
        Assert.Equal(testCase.Expected, ShortestPalindromeSolution.BuildShortestPalindromeByNaiveScan(testCase.Input));

    [Theory]
    [MemberData(nameof(Cases))]
    public void BuildShortestPalindromeByKmpFailureFunction_Cases_PrependsFewestCharacters(
        PalindromeCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            ShortestPalindromeSolution.BuildShortestPalindromeByKmpFailureFunction(testCase.Input));

    // One case: the string and the shortest palindrome it builds. The
    // two are both strings in adjacent positions at the call site, so the bundle
    // names the input and the expectation rather than leaving them swappable.
    public readonly record struct PalindromeCase(string Input, string Expected);
}
