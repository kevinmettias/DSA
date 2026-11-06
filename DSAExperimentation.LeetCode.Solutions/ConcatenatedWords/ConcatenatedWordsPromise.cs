namespace DSAExperimentation.LeetCode.Solutions.ConcatenatedWords;

// LC 472's promise about its input: the words are unique and hold lowercase English letters only. LeetCode's
// judge never poses an input that breaks it. A word given twice is a concatenation of itself to one arm and
// not to the other, which builds its dictionary from the other words alone. Generated cases do not always keep
// the promise, so a harness that runs them asks this first.
internal static class ConcatenatedWordsPromise
{
    public static bool IsKeptBy(string[] words)
    {
        var isEachWordLowercase = words.All(word => word.All(char.IsAsciiLetterLower));

        return isEachWordLowercase && words.Distinct().Count() == words.Length;
    }
}
