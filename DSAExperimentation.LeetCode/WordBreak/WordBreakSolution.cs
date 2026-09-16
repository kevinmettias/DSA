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
    public static bool CanBreakByTrieMemoized(string source, IList<string> wordDict)
    {
        var trie = new Trie<bool>();

        foreach (var word in wordDict)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, bool>(0, new SegmentableFromEveryIndex(source, trie));
    }

    // The recurrence, named: source[start..] segments when some prefix of it is a whole
    // dictionary word and the remainder behind that word segments too, with the
    // exhausted string as the base case. The string and the trie screening its
    // prefixes belong to the caller and never vary during a run, so they travel in
    // as constructor state.
    private sealed class SegmentableFromEveryIndex(string source, Trie<bool> trie)
        : IRecurrence<int, bool>
    {
        /// <inheritdoc/>
        public bool Replay(int start, IRecurrence<int, bool> rest)
        {
            if (start == source.Length)
            {
                return true;
            }

            for (var end = start + 1; end <= source.Length; end++)
            {
                var piece = source[start..end];

                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (trie.HasKey(piece) && rest.Replay(end, rest))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
