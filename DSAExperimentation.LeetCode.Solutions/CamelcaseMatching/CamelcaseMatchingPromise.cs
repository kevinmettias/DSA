namespace DSAExperimentation.LeetCode.Solutions.CamelcaseMatching;

// LC 1023's promise about its input: every query and the pattern consist of English letters.
// LeetCode's judge never poses an input that breaks it. The two-pointer scan inserts only lowercase
// letters, and a digit is neither case, so the strategies part on one. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class CamelcaseMatchingPromise
{
    public static bool IsKeptBy(string[] queries, string pattern)
        => queries.Append(pattern).All(word => word.All(char.IsAsciiLetter));
}
