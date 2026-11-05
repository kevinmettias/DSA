namespace DSAExperimentation.LeetCode.Solutions.LongestPalindromicSubstring;

// LC 5 accepts any longest palindromic substring, as its first example says ("aba" is also a valid
// answer). The catalog records one, so an answer stands when it is a palindrome found in the string and
// as long as the recorded one. Of two that tie, the center expansion keeps the first it meets and
// Manacher's the one its radii end on.
internal static class LongestPalindromicSubstringAnswerCheck
{
    public static bool IsAccepted(string text, string answer, string recorded)
        => answer.Length == recorded.Length && text.Contains(answer, StringComparison.Ordinal) && answer.SequenceEqual(answer.Reverse());
}
