using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.ShortEncodingOfWords;

// LeetCode 820. Short Encoding of Words: the length of the shortest reference
// string in which every word appears as a "#"-terminated run. A word needs its own
// entry only when no other, longer word already ends with it - so the answer is
// the summed length + 1 of every distinct word that is not a proper suffix of
// another.
//
// Three ways to answer "is this word a proper suffix of another": check every pair
// with EndsWith, evict every proper suffix from a Set as each word is scanned, or
// insert every REVERSED word into a LowercaseTrie and keep only the words whose
// reversed node is a leaf.
internal static class ShortEncodingOfWordsSolution
{
    // Every kept word costs its own characters plus the '#' that terminates it.
    private const int SeparatorLength = 1;

    // The textbook answer: O(n^2 * L) pairwise EndsWith over the distinct words,
    // with BCL structures throughout - it is the arm the two composed strategies
    // below have to justify themselves against.
    public static int MinimumLengthByPairwiseSuffixScan(string[] words)
    {
        var distinctWords = new List<string>();
        var seen = new HashSet<string>();

        foreach (var word in words)
        {
            if (seen.Add(word))
            {
                distinctWords.Add(word);
            }
        }

        var length = 0;

        for (var i = 0; i < distinctWords.Count; i++)
        {
            if (!IsSuffixOfAnother(distinctWords, i))
            {
                length += distinctWords[i].Length + SeparatorLength;
            }
        }

        return length;
    }

    private static bool IsSuffixOfAnother(List<string> distinctWords, int index)
    {
        var word = distinctWords[index];

        for (var other = 0; other < distinctWords.Count; other++)
        {
            var candidate = distinctWords[other];

            if (IsProperSuffixOf(new SuffixCandidate(word), new ContainingWord(candidate), index, other))
            {
                return true;
            }
        }

        return false;
    }

    // A word is a proper suffix of another when some different, longer word ends
    // with it. The two sides are distinct roles - one is the tail being looked for
    // and the other is the word that would have to end with it - so they are named
    // rather than left as two `string` positions a caller could hand over swapped.
    private static bool IsProperSuffixOf(
        SuffixCandidate word, ContainingWord otherWord, int index, int otherIndex)
        => otherIndex != index
            && otherWord.Text.Length > word.Text.Length
            && otherWord.Text.EndsWith(word.Text, StringComparison.Ordinal);

    // This repo's own Set<string>: every word starts as a survivor, then every one
    // of its proper suffixes is evicted the instant a longer word is found to
    // contain it. Whatever survives is exactly the set of words that need their
    // own encoding.
    public static int MinimumLengthBySuffixEviction(string[] words)
    {
        var distinctWords = Deduplicate(words);
        var remaining = FindWordsThatAreNotSuffixesOfAnother(distinctWords);
        var length = 0;

        foreach (var word in distinctWords)
        {
            if (remaining.Has(word))
            {
                length += word.Length + SeparatorLength;
            }
        }

        return length;
    }

    private static Set<string> FindWordsThatAreNotSuffixesOfAnother(List<string> distinctWords)
    {
        var remaining = new Set<string>(distinctWords);

        foreach (var word in distinctWords)
        {
            for (var i = 1; i < word.Length; i++)
            {
                var suffix = word[i..];
                remaining.TryRemove(suffix);
            }
        }

        return remaining;
    }

    // This repo's own bounded-alphabet LowercaseTrie<bool>, fed every word
    // REVERSED: a word needs its own encoding exactly when no other reversed word
    // extends past it, i.e. when its reversed node is a trie leaf. O(total
    // characters) instead of the pairwise scan's O(n^2 * L).
    public static int MinimumLengthByReversedTrieLeaves(string[] words)
    {
        var distinctWords = Deduplicate(words);
        var trie = BuildReversedTrie(distinctWords);
        var length = 0;

        foreach (var word in distinctWords)
        {
            var node = WalkReversed(trie.Root, word);

            if (IsLeaf(node))
            {
                length += word.Length + SeparatorLength;
            }
        }

        return length;
    }

    private static LowercaseTrie<bool> BuildReversedTrie(List<string> distinctWords)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in distinctWords)
        {
            var reversed = Reverse(word);
            trie.Set(reversed, true);
        }

        return trie;
    }

    private static LowercaseTrieNode<bool> WalkReversed(LowercaseTrieNode<bool> root, string word)
    {
        var node = root;

        for (var i = word.Length - 1; i >= 0; i--)
        {
            node = node.Children[word[i] - 'a']!;
        }

        return node;
    }

    private static bool IsLeaf(LowercaseTrieNode<bool> node)
    {
        foreach (var child in node.Children)
        {
            if (child is not null)
            {
                return false;
            }
        }

        return true;
    }

    private static string Reverse(string word)
    {
        var characters = word.ToCharArray();
        Array.Reverse(characters);

        return new string(characters);
    }

    // A repeated word is encoded once, so duplicates are dropped up front - both
    // the survivor set and the trie would otherwise charge the same word twice.
    private static List<string> Deduplicate(string[] words)
    {
        var distinctWords = new List<string>();
        var seen = new Set<string>();

        foreach (var word in words)
        {
            if (seen.TryAdd(word))
            {
                distinctWords.Add(word);
            }
        }

        return distinctWords;
    }

    // The tail a word is being tested as, versus the word that would have to end with it: two
    // one-directional roles that used to be two interchangeable `string` positions.
    private readonly record struct SuffixCandidate(string Text);

    private readonly record struct ContainingWord(string Text);
}
