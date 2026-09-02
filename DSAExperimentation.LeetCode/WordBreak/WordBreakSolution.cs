using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.WordBreak;

// LeetCode 139. Word Break: can s be segmented into a space-separated sequence of
// one or more dictionary words?
//
// This repo's own Trie<bool> screens dictionary prefixes; Memoizer caches each
// start index's segmentability so a shared suffix is only solved once - the same
// composition this repo's Word Break II coverage extends to collect every sentence
// instead of a single true/false.
internal static class WordBreakSolution
{
    public static bool CanBreakByTrieMemoized(string s, IList<string> wordDict)
    {
        var trie = new Trie<bool>();

        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, bool>(0, (start, canSegment) => TrieMemoizedFrom(s, trie, start, canSegment));
    }

    private static bool TrieMemoizedFrom(string s, Trie<bool> trie, int start, Func<int, bool> canSegment)
    {
        if (start == s.Length)
        {
            return true;
        }

        for (var end = start + 1; end <= s.Length; end++)
        {
            var piece = s[start..end];

            if (!trie.HasPrefix(piece))
            {
                break;
            }

            if (trie.HasKey(piece) && canSegment(end))
            {
                return true;
            }
        }

        return false;
    }
}
