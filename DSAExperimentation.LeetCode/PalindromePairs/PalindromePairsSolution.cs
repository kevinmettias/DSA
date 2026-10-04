using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PalindromePairs;

// LeetCode 336. Palindrome Pairs: for every ordered pair of distinct words, does
// words[i] + words[j] read the same forwards and backwards? The statement requires
// O(sum of words[i].length) time, and only FindPairsByReversedWordTrie meets it.
//
// FindPairsByBruteForce checks every ordered pair directly by concatenating and
// scanning, O(n^2*k). FindPairsByHashMapComplementLookup instead asks, for every
// prefix/suffix split of one word, whether the exact reversed complement is a
// distinct word in the list - the same complement-lookup shape TwoSum uses, applied
// to string prefixes/suffixes instead of numeric complements - using this repo's own
// HashMap<TKey,TValue> for the O(1) lookup, in O(n*k^2) (k = max word length): every
// split builds, reverses and hashes its sides afresh. FindPairsByReversedWordTrie
// walks each word once down this repo's LowercaseTrie of the words reversed, asking
// Manacher's radii whether the rest of the word is a palindrome at each step.
internal static class PalindromePairsSolution
{
    // The textbook O(n^2*k) answer: try every ordered pair, concatenate, and scan
    // for a palindrome. Deliberately the naive baseline the complement lookup below
    // has to justify itself against.
    public static List<(int First, int Second)> FindPairsByBruteForce(IReadOnlyList<string> words)
    {
        var pairs = new List<(int, int)>();

        for (var i = 0; i < words.Count; i++)
        {
            for (var j = 0; j < words.Count; j++)
            {
                if (i != j && IsPalindrome(words[i] + words[j]))
                {
                    pairs.Add((i, j));
                }
            }
        }

        return pairs;
    }

    // For every word and every split point, does the exact reversed complement of
    // one side exist elsewhere in the list, with the other side already a
    // palindrome? One O(1) HashMap lookup per split answers it.
    public static List<(int First, int Second)> FindPairsByHashMapComplementLookup(IReadOnlyList<string> words)
    {
        var indexOf = new HashMap<string, int>();

        for (var i = 0; i < words.Count; i++)
        {
            indexOf.Set(words[i], i);
        }

        var pairs = new List<(int, int)>();

        for (var i = 0; i < words.Count; i++)
        {
            var word = words[i];

            for (var cut = 0; cut <= word.Length; cut++)
            {
                AddPairsAtCut((word, i), cut, indexOf, pairs);
            }
        }

        return pairs;
    }

    private static void AddPairsAtCut(
        (string Word, int Index) entry, int cut, HashMap<string, int> indexOf, List<(int, int)> pairs)
    {
        var prefix = entry.Word[..cut];
        var suffix = entry.Word[cut..];
        var suffixMatch = FindComplementIndex(indexOf, suffix);

        if (IsPalindrome(prefix) && IsAnotherWord(suffixMatch, entry.Index))
        {
            pairs.Add((suffixMatch, entry.Index));
        }

        var prefixMatch = FindComplementIndex(indexOf, prefix);

        if (HasPalindromicRightSide(entry.Word, cut, suffix) && IsAnotherWord(prefixMatch, entry.Index))
        {
            pairs.Add((entry.Index, prefixMatch));
        }
    }

    // Where the reversed complement of one side sits in the list, or -1 when no such
    // word is present.
    private static int FindComplementIndex(HashMap<string, int> indexOf, string side)
    {
        if (indexOf.TryGetValue(Reverse(side), out var match))
        {
            return match;
        }

        return -1;
    }

    // A pair needs two distinct words, so the complement found must be another one.
    private static bool IsAnotherWord(int matchIndex, int wordIndex)
        => matchIndex >= 0 && matchIndex != wordIndex;

    // The cut leaves a right-hand side, and that side already reads as a palindrome,
    // so a reversed complement on the left completes the pair.
    private static bool HasPalindromicRightSide(string word, int cut, string suffix)
        => cut != word.Length && IsPalindrome(suffix);

    // O(sum of words[i].length), the bound LC 336 sets, over one of this repo's
    // LowercaseTries holding every word reversed. words[i] + words[j] is a palindrome
    // in exactly one of two ways:
    // - words[j] is no longer than words[i]: words[j] reversed opens words[i], and the
    //   rest of words[i] is a palindrome (empty when the lengths match).
    // - words[j] is longer: words[i] opens words[j] reversed, and the rest of words[j]
    //   reversed is a palindrome - that is, the front of words[j] is.
    // Inserting words[j] reversed records it at the node where it ends, and on its way
    // down at every node where the rest of it is a palindrome. Walking words[i] down
    // the trie then meets each pair of the first kind at the node its partner ends on,
    // and every pair of the second kind at once, listed at the node where words[i]
    // runs out. Each insertion and each walk takes one step per letter, and each step
    // asks whether a span of the word is a palindrome in O(1), from Manacher's radii
    // computed once per word in O(its length). A word records at most one entry per
    // letter, and only the walk spelling a node exactly reads its list, so the lists
    // are written and read in O(total length); at most one word ends at any node, the
    // words being unique, and no pair is of both kinds, so none is reported twice. Each
    // new node's 26-slot child array is a constant of the alphabet, and n is at most
    // the total length plus one, since only one word can be empty.
    //
    // One trie, rather than a second over the words as written for the second kind: the
    // lists cost a few objects per word where a second trie doubles every node, and on
    // 400 words of up to 300 letters that pushed the run into gen-2 collections and took
    // about seven times as long.
    //
    // It walks Root/Children itself, as CountPrefixAndSuffixPairsIISolution does: a node's
    // Value here is the WordStop recorded there, on inner nodes too, which LowercaseTrie's
    // own Set - one value at the end of each key - has no way to write.
    public static List<(int First, int Second)> FindPairsByReversedWordTrie(IReadOnlyList<string> words)
    {
        var reversedWords = new LowercaseTrie<WordStop>();
        var indexedWords = new IndexedWord[words.Count];

        for (var i = 0; i < words.Count; i++)
        {
            indexedWords[i] = new IndexedWord(words[i], i, PalindromicSpans.Of(words[i]));
            InsertReversed(reversedWords.Root, indexedWords[i]);
        }

        var pairs = new List<(int, int)>();

        foreach (var word in indexedWords)
        {
            AddPairsWithWordOnLeft(reversedWords.Root, word, pairs);
        }

        return pairs;
    }

    // The word's letters from last to first: at depth d the reversal's remaining letters
    // are the word's first length - d, so it is listed there when those are a palindrome.
    private static void InsertReversed(LowercaseTrieNode<WordStop> root, IndexedWord word)
    {
        var node = root;
        var lastLetter = word.Text.Length - 1;

        for (var depth = 0; depth < word.Text.Length; depth++)
        {
            if (word.Spans.IsPalindrome(0, word.Text.Length - depth))
            {
                StopAt(node).LongerWords.Add(word.Index);
            }

            node = ChildOrNew(node, word.Text[lastLetter - depth]);
        }

        StopAt(node).EndingWord = word.Index;
    }

    // The node's stop, created the first time anything is recorded there. HasValue marks
    // the nodes that have one, as LowercaseTrie's own Set marks the nodes holding a value.
    private static WordStop StopAt(LowercaseTrieNode<WordStop> node)
    {
        if (!node.HasValue)
        {
            node.Value = new WordStop();
            node.HasValue = true;
        }

        return node.Value;
    }

    private static LowercaseTrieNode<WordStop> ChildOrNew(LowercaseTrieNode<WordStop> node, char letter)
        => node.Children[letter - 'a'] ??= new LowercaseTrieNode<WordStop>();

    // The word on the left. A node at depth d below the word's own length spells its
    // first d letters, so a word ending there is them reversed and shorter, and pairs
    // when the word's remaining letters are a palindrome. The node spelling the whole
    // word holds the rest: the word of equal length ending there, and the longer ones.
    private static void AddPairsWithWordOnLeft(
        LowercaseTrieNode<WordStop> root, IndexedWord word, List<(int, int)> pairs)
    {
        LowercaseTrieNode<WordStop>? node = root;

        for (var depth = 0; node is not null && depth < word.Text.Length; depth++)
        {
            if (node.HasValue && node.Value.EndingWord is int shorter
                && word.Spans.IsPalindrome(depth, word.Text.Length - depth))
            {
                pairs.Add((word.Index, shorter));
            }

            node = ChildFor(node, word.Text[depth]);
        }

        if (node is { HasValue: true })
        {
            AddPairsWhereWordEnds(node.Value, word.Index, pairs);
        }
    }

    // The node one letter further down, or null where no reversed word continues with it.
    private static LowercaseTrieNode<WordStop>? ChildFor(LowercaseTrieNode<WordStop> node, char letter)
        => node.Children[letter - 'a'];

    // At the node spelling the whole left word, the word ending there is its own reversal
    // - itself, when it is a palindrome, and then no pair - and every longer word listed
    // there pairs.
    private static void AddPairsWhereWordEnds(WordStop stop, int wordIndex, List<(int, int)> pairs)
    {
        if (stop.EndingWord is int sameLength && sameLength != wordIndex)
        {
            pairs.Add((wordIndex, sameLength));
        }

        foreach (var longer in stop.LongerWords)
        {
            pairs.Add((wordIndex, longer));
        }
    }

    private static bool IsPalindrome(string text)
    {
        var left = 0;
        var right = text.Length - 1;

        while (left < right)
        {
            if (text[left++] != text[right--])
            {
                return false;
            }
        }

        return true;
    }

    private static string Reverse(string text)
    {
        var chars = text.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    // One word as the trie sees it: its letters, its index in the list, and the
    // palindrome tests over its spans.
    private readonly record struct IndexedWord(string Text, int Index, PalindromicSpans Spans);

    // What a node of the reversed-word trie records, on the nodes that record anything:
    // the word whose reversal ends here, and every longer word whose reversal runs on
    // past here with a palindrome left over.
    private sealed class WordStop
    {
        public int? EndingWord { get; set; }

        public List<int> LongerWords { get; } = [];
    }

    // Whether any span of one word is a palindrome, in O(1) each, from Manacher's two
    // radius arrays computed once in O(the word's length). A span of odd length is a
    // palindrome when the odd radius at its middle letter reaches both of its ends; one
    // of even length, when the even radius at the position just right of its middle does.
    private readonly struct PalindromicSpans(int[] oddRadii, int[] evenRadii)
    {
        private const int Sides = 2;

        public static PalindromicSpans Of(string text) =>
            new(Manacher.ComputeOddRadii(text), Manacher.ComputeEvenRadii(text));

        public bool IsPalindrome(int start, int length)
        {
            if (length == 0)
            {
                return true;
            }

            var halfLength = length / Sides;
            var middle = start + halfLength;

            if (length % Sides == 0)
            {
                return evenRadii[middle] >= halfLength;
            }

            return oddRadii[middle] > halfLength;
        }
    }
}
