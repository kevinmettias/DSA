using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.ShortestPalindrome;

// LeetCode 214. Shortest Palindrome: find the fewest characters that can be
// prepended to the text so the whole string reads as a palindrome. Equivalently,
// find the text's longest palindromic PREFIX and prepend the reverse of whatever
// is left over.
//
// The two strategies differ only in how they find that longest palindromic
// prefix - an O(n^2) double-ended scan trying every prefix length, or building
// text + '#' + reverse(text) and reading the answer off the last entry of this
// repo's own KMP failure function (PrefixFunctionSearch.ComputeFailureFunction):
// the longest run that is simultaneously a prefix of text and a suffix of
// reverse(text) is exactly the text's longest palindromic prefix.
internal static class ShortestPalindromeSolution
{
    private const string FailureFunctionSeparator = "#";

    // The textbook answer: for each candidate prefix length, walk inward from
    // both ends checking equality. Deliberately written without this repo's
    // primitives - it is the arm the KMP strategy has to justify itself against.
    public static string BuildShortestPalindromeByNaiveScan(string text)
    {
        for (var end = text.Length; end > 0; end--)
        {
            if (IsPalindromePrefix(text, end))
            {
                var suffix = text[end..];
                return new string(suffix.Reverse().ToArray()) + text;
            }
        }

        return text;
    }

    private static bool IsPalindromePrefix(string text, int length)
    {
        var left = 0;
        var right = length - 1;

        while (left < right)
        {
            if (text[left] != text[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    public static string BuildShortestPalindromeByKmpFailureFunction(string text)
    {
        if (text.Length == 0)
        {
            return text;
        }

        var reversed = new string(text.Reverse().ToArray());
        var combined = text + FailureFunctionSeparator + reversed;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(combined);
        var longestPalindromicPrefix = failure[^1];

        return new string(text[longestPalindromicPrefix..].Reverse().ToArray()) + text;
    }
}
