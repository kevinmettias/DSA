namespace DSAExperimentation.LeetCode.Solutions.WordsWithinTwoEditsOfDictionary;

// LC 2452's promise about its input: every query and every dictionary word has one length, n. LeetCode's
// judge never poses an input that breaks it. The brute force compares letters position by position and
// runs past the shorter word, and the edit-budget trie never matches words of different lengths.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class WordsWithinTwoEditsOfDictionaryPromise
{
    public static bool IsKeptBy(string[] queries, string[] dictionary) => queries.Concat(dictionary).Select(word => word.Length).Distinct().Count() <= 1;
}
