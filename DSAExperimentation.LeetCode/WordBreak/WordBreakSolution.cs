using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.WordBreak;

// LeetCode 139. Word Break: can s be segmented into a space-separated sequence of
// one or more dictionary words?
//
// Two strategies sit here. CanBreakByTrieMemoized composes this repo's own
// Trie<bool> - screening dictionary prefixes - with Memoizer, which caches each
// start index's segmentability so a shared suffix is only solved once; the same
// composition this repo's Word Break II coverage extends to collect every sentence
// instead of a single true/false. CanBreakByIterativeReachability rolls that
// segmentability forward instead, marking every position a dictionary word reaches
// from an already-reachable one.
internal static class WordBreakSolution
{
    // The textbook arm the Trie + Memoizer composition is measured against: a
    // bottom-up sweep marking every position reachable by a dictionary word from an
    // already-reachable start, with a HashSet for whole-word membership. It slices
    // the source at every boundary rather than screening increments through the
    // trie, and pays no Memoizer traffic.
    public static bool CanBreakByIterativeReachability(string source, IList<string> wordDict)
    {
        var words = new HashSet<string>(wordDict);
        var reachable = new bool[source.Length + 1];
        reachable[0] = true;

        for (var start = 0; start < source.Length; start++)
        {
            if (!reachable[start])
            {
                continue;
            }

            for (var end = start + 1; end <= source.Length; end++)
            {
                if (words.Contains(source[start..end]))
                {
                    reachable[end] = true;
                }
            }
        }

        return reachable[^1];
    }

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
