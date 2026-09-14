using DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubstrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfTheLengthOfTwoPalindromicSubstrings;

// Harness only. Both strategies are
// MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution's - the Manacher-radii
// sweep and the O(n^3) expand-around-every-center scan that used to live untested as
// the benchmark's baseline - pinned to LeetCode's published examples plus the cases
// that separate them: a whole string that is itself a palindrome (so the answer must
// come from nested sub-palindromes), a run of one repeated character (where the best
// split is not the middle), and inputs whose only answer is 1 * 1.
public sealed class MaximumProductOfTheLengthOfTwoPalindromicSubstringsTests
{
    public static TheoryData<string, long> Examples =>
        new()
        {
            { "ababbb", 9L }, // "aba" (0..2) * "bbb" (3..5)
            { "zaaaxbbby", 9L }, // "aaa" * "bbb"
            { "aaaaa", 3L }, // "aaa" (a nested sub-palindrome, not the whole string) * "a"
            { "aa", 1L }, // one character each side is all that fits
            { "abc", 1L }, // no palindrome longer than a single character
            { "dddaaa", 9L }, // "ddd" * "aaa", the split exactly between them
            { "abacaba", 9L }, // the whole string is a palindrome; two nested "aba"s beat it
            { "aaaaaaaaa", 15L }, // best split is 3 * 5, not the even halves
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByManacherRadii_LeetCodeAndNestedPalindromeExamples_ReturnsBestSplitProduct(
        string s, long expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution.MaxProductByManacherRadii(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByCenterExpansion_LeetCodeAndNestedPalindromeExamples_ReturnsBestSplitProduct(
        string s, long expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution.MaxProductByCenterExpansion(s));
}
