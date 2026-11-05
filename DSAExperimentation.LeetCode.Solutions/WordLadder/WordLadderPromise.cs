namespace DSAExperimentation.LeetCode.Solutions.WordLadder;

// LC 127's promise about its input: all the words in the word list are unique. LeetCode's judge never poses
// an input that breaks it. The graph build adds each word as a node once and throws on a second. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class WordLadderPromise
{
    public static bool IsKeptBy(BeginWord beginWord, EndWord endWord, IEnumerable<string> wordList)
        => wordList.Distinct().Count() == wordList.Count();
}
