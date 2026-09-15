using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsII;

// LeetCode 3045. Count Prefix and Suffix Pairs II: count index pairs (i < j)
// where words[i] is both a PREFIX and a SUFFIX of words[j] - the same question
// CountPrefixAndSuffixPairsISolution answers, but words.length and total
// length are now large enough (up to 1e5 / 5e5) that I's O(n^2 * L) brute
// force, even with O(1) RollingHash comparisons per pair, cannot finish in
// time: RollingHash still checks every pair, and here there are too many
// pairs to check at all.
//
// The fix is to never check a pair directly. words[i] is a prefix-and-suffix
// of words[j] iff, for every position k < len(i), words[i][k] == words[j][k]
// (the prefix condition) AND words[i][len(i)-1-k] == words[j][len(j)-1-k]
// (the suffix condition, reading both strings from their own ends). Encode
// each word as the interleaved sequence (word[0], word[len-1], word[1],
// word[len-2], ...) and that pair of conditions becomes exactly "words[i]'s
// encoded sequence is a prefix of words[j]'s encoded sequence" - one walk
// down a shared trie, so inserting word j and summing the counter at every
// node visited along the way (each counter = how many earlier words ended
// exactly there) counts every earlier word that is a prefix-and-suffix of
// word j in O(len(j)) total, no re-walk from the root per position.
internal static class CountPrefixAndSuffixPairsIISolution
{
    // The textbook O(n^2 * L) scan, unchanged from CountPrefixAndSuffixPairsI -
    // correct at any size, just the arm the trie strategy below has to beat
    // once n and L grow past what pairwise comparison can finish in time.
    public static long CountPairsByBruteForce(string[] words)
    {
        var count = 0L;

        for (var i = 0; i < words.Length; i++)
        {
            for (var j = i + 1; j < words.Length; j++)
            {
                if (IsPrefixAndSuffix(new PrefixSuffixCandidate(words[i]), new ContainingWord(words[j])))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsPrefixAndSuffix(PrefixSuffixCandidate candidate, ContainingWord word)
    {
        if (candidate.Text.Length > word.Text.Length)
        {
            return false;
        }

        return word.Text.AsSpan(0, candidate.Text.Length).SequenceEqual(candidate.Text) &&
               word.Text.AsSpan(word.Text.Length - candidate.Text.Length).SequenceEqual(candidate.Text);
    }

    // This repo's own LowercaseTrie<TValue> (Graph/Engines/Dags/Trees), keyed
    // one interleaved character at a time via its public Root/Children/
    // Value - the same direct node-walking LowercaseTrieTests already relies
    // on for its own longest-common-prefix composition. Value doubles as a
    // per-node counter (how many words' encoded sequence ends exactly here)
    // rather than the single-value slot LowercaseTrie.Set assumes, which is
    // why this walks Root/Children itself instead of calling Set/TryGetValue.
    public static long CountPairsByLowercaseTrie(string[] words)
    {
        var trie = new LowercaseTrie<int>();
        var pairs = 0L;

        foreach (var word in words)
        {
            var current = trie.Root;
            var length = word.Length;

            for (var position = 0; position < length; position++)
            {
                current = StepInto(current, word[position]);
                pairs += current.Value;

                current = StepInto(current, word[length - 1 - position]);
                pairs += current.Value;
            }

            current.HasValue = true;
            current.Value++;
        }

        return pairs;
    }

    private static LowercaseTrieNode<int> StepInto(LowercaseTrieNode<int> node, char letter)
        => node.Children[letter - 'a'] ??= new LowercaseTrieNode<int>();

    // The two sides of "is words[i] a prefix-and-suffix of words[j]": the candidate that has
    // to appear at both ends, and the word it has to appear in. The relation is one-directional,
    // so the two `string` positions they used to be were a swap the compiler would have allowed.
    private readonly record struct PrefixSuffixCandidate(string Text);

    private readonly record struct ContainingWord(string Text);
}
