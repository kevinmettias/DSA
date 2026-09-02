using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.ShortestPalindrome;

// LeetCode 214. Shortest Palindrome: find the fewest characters that can be
// prepended to s so the whole string reads as a palindrome. Equivalently, find
// s's longest palindromic PREFIX and prepend the reverse of whatever is left over.
//
// The two strategies differ only in how they find that longest palindromic
// prefix - an O(n^2) double-ended scan trying every prefix length, or building
// s + '#' + reverse(s) and reading the answer off the last entry of this repo's
// own KMP failure function (PrefixFunctionSearch.ComputeFailureFunction): the
// longest run that is simultaneously a prefix of s and a suffix of reverse(s) is
// exactly s's longest palindromic prefix.
internal static class ShortestPalindromeSolution
{
    private const string FailureFunctionSeparator = "#";

    // The textbook answer: for each candidate prefix length, walk inward from
    // both ends checking equality. Deliberately written without this repo's
    // primitives - it is the arm the KMP strategy has to justify itself against.
    public static string BuildShortestPalindromeByNaiveScan(string s)
    {
        for (var end = s.Length; end > 0; end--)
        {
            if (IsPalindromePrefix(s, end))
            {
                var suffix = s[end..];
                return new string(suffix.Reverse().ToArray()) + s;
            }
        }

        return s;
    }

    private static bool IsPalindromePrefix(string s, int length)
    {
        var left = 0;
        var right = length - 1;

        while (left < right)
        {
            if (s[left] != s[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    public static string BuildShortestPalindromeByKmpFailureFunction(string s)
    {
        if (s.Length == 0)
        {
            return s;
        }

        var reversed = new string(s.Reverse().ToArray());
        var combined = s + FailureFunctionSeparator + reversed;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(combined);
        var longestPalindromicPrefix = failure[^1];

        return new string(s[longestPalindromicPrefix..].Reverse().ToArray()) + s;
    }
}
