namespace DSAExperimentation.LeetCode.Solutions.LongestCommonSuffixQueries;

// LC 3093's promise about its input: every word in the container and every query consists of
// lowercase English letters. LeetCode's judge never poses an input that breaks it, and the trie arm
// leans on it - its children are slots over the lowercase letters, so a word with any other character
// has no path to take. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class LongestCommonSuffixQueriesPromise
{
    public static bool IsKeptBy(string[] wordsContainer, string[] wordsQuery)
        => wordsContainer.Concat(wordsQuery).All(word => word.All(char.IsAsciiLetterLower));
}
