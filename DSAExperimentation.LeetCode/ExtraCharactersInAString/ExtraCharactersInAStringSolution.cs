using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.ExtraCharactersInAString;

// LeetCode 2707. Extra Characters in a String: break text into non-overlapping pieces
// that are all dictionary words and report the fewest characters left over. Both
// strategies fill the same recurrence - the best from a start position is either
// "waste this character and continue" or "consume a dictionary word starting here and
// continue past it" - so what separates them is only how a start position discovers
// the dictionary words that begin there:
//
//   HashSetFullScan is the textbook arm. It walks every end position from start to the
//   end of the string, cuts the substring out and hashes it against a HashSet<string>,
//   paying an O(remaining length) substring extraction per candidate even when no
//   dictionary word could possibly begin with that prefix. O(n^2) substrings, filled
//   bottom-up over an int[]. Deliberately all BCL internally - it is what you would
//   write without this repo, and it lives here rather than in the benchmark so that it
//   is actually asserted.
//
//   TriePrunedScan walks a Trie<bool> instead, extending the candidate one character at
//   a time and stopping the instant HasPrefix reports that no dictionary word shares
//   that prefix. The inner loop is then bounded by the longest dictionary word rather
//   than by the remaining string length. The recurrence itself is Memoizer.Memoize over
//   the start index - the same Trie + Memoizer segmentation WordBreak uses for LC 139,
//   minimizing a leftover count instead of answering a yes/no reachability.
internal static class ExtraCharactersInAStringSolution
{
    // The naive arm: hash every substring against the dictionary. Internals are
    // deliberately all BCL.
    public static int MinExtraCharsByHashSetFullScan(string text, string[] dictionary)
    {
        var words = new HashSet<string>(dictionary);
        var length = text.Length;
        var fewestLeftoverFrom = new int[length + 1];

        for (var start = length - 1; start >= 0; start--)
        {
            var best = 1 + fewestLeftoverFrom[start + 1];

            for (var end = start + 1; end <= length; end++)
            {
                if (words.Contains(text[start..end]))
                {
                    best = Math.Min(best, fewestLeftoverFrom[end]);
                }
            }

            fewestLeftoverFrom[start] = best;
        }

        return fewestLeftoverFrom[0];
    }

    // This repo's own arm: a Trie<bool> prunes the candidate scan the moment the prefix
    // leaves the dictionary, and Memoizer carries the recurrence over start indices.
    public static int MinExtraCharsByTriePrunedScan(string text, string[] dictionary)
    {
        var words = BuildTrie(dictionary);

        return FewestLeftoverByTrie(text, words);
    }

    private static Trie<bool> BuildTrie(string[] dictionary)
    {
        var words = new Trie<bool>();

        foreach (var word in dictionary)
        {
            words.Set(word, true);
        }

        return words;
    }

    // The recurrence the memoized arm fills, with the trie handed in already built:
    // from a start position the fewest leftover characters are either "waste this one
    // and continue" or the best over the dictionary words that begin here.
    private static int FewestLeftoverByTrie(string text, Trie<bool> words) =>
        Memoizer.Memoize<int, int>(0, new FewestLeftoverFromStart(text, words));

    // The end positions of the dictionary words that begin at `start`, in ascending
    // order, giving up as soon as the candidate leaves the dictionary: the same
    // Trie<bool> walk, just handed to the recurrence one word at a time so the
    // recurrence itself reads as the two branches it chooses between.
    private static IEnumerable<int> WordEndsStartingAt(string text, Trie<bool> words, int start)
    {
        for (var end = start + 1; end <= text.Length; end++)
        {
            var piece = text[start..end];
            if (!words.HasPrefix(piece))
            {
                yield break;
            }

            if (words.HasKey(piece))
            {
                yield return end;
            }
        }
    }

    // The recurrence, as a named type: from a start position the fewest leftover
    // characters are either this one wasted and the rest continued, or the best over
    // the dictionary words that begin right here.
    private sealed class FewestLeftoverFromStart(
        string text, Trie<bool> words) : IRecurrence<int, int>
    {
        public int Replay(int start, IRecurrence<int, int> rest)
        {
            if (start == text.Length)
            {
                return 0;
            }

            var best = 1 + rest.Replay(start + 1, rest);

            foreach (var end in WordEndsStartingAt(text, words, start))
            {
                var afterWord = rest.Replay(end, rest);
                best = Math.Min(best, afterWord);
            }

            return best;
        }
    }
}
