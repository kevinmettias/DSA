namespace DSAExperimentation.LeetCode.Solutions.LongestDuplicateSubstring;

// LC 1044 accepts any duplicated substring of the longest length, and "" when none repeats. The catalog
// records one, so an answer stands when it is as long as the recorded one and occurs at least twice,
// overlapping or not. Of two that tie, the pair scan and the suffix array each keep the first they meet.
internal static class LongestDuplicateSubstringAnswerCheck
{
    public static bool IsAccepted(string text, string answer, string recorded)
        => answer.Length == recorded.Length && (answer.Length == 0 || text.IndexOf(answer, StringComparison.Ordinal) != text.LastIndexOf(answer, StringComparison.Ordinal));
}
