namespace DSAExperimentation.LeetCode.Solutions.PalindromePairs;

// LC 336's promise about its input: the words are unique ("a 0-indexed array of unique strings") and hold
// lowercase English letters only. LeetCode's judge never poses an input that breaks it. The trie arm keeps a
// slot per lowercase letter, so a word with a digit throws there, and a word given twice pairs with itself under
// one arm's lookup and not another's. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class PalindromePairsPromise
{
    public static bool IsKeptBy(IReadOnlyList<string> words)
    {
        var unique = words.ToHashSet();

        return unique.Count == words.Count && words.All(word => word.All(char.IsAsciiLetterLower));
    }
}
