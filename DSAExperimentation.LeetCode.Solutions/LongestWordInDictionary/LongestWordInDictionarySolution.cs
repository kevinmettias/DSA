using DSAExperimentation.DataStructures;
using DSAExperimentation.Domain.Language.English;
using WordNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrieNode<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    bool>;
using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrie<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.LongestWordInDictionary;

// LeetCode 720. Longest Word in Dictionary: find the longest word buildable one
// character at a time from other words in the list (ties broken lexicographically
// smallest), where "buildable" means every prefix of the word is itself a complete
// word in the list.
//
// The two strategies differ in how "is this prefix a complete word" is answered -
// a linear scan of the whole word list per prefix, or a single O(1) HasValue check
// per step of one root-to-leaf walk over this repo's own AlphabetTrie.
internal static class LongestWordInDictionarySolution
{
    private const string NoQualifyingWord = "";

    // The textbook answer: for every word, verify each of its prefixes exists
    // ANYWHERE in the word list via a linear scan - no index at all.
    // O(words^2 * wordLength) vs. the trie walk's O(totalCharacters).
    public static string LongestWordByDictionaryScan(string[] words)
    {
        string? best = null;

        foreach (var word in words)
        {
            if (!IsBuildable(word, words))
            {
                continue;
            }

            if (best is null || IsBetter(new CandidateWord(word), new BestWord(best)))
            {
                best = word;
            }
        }

        return best ?? NoQualifyingWord;
    }

    private static bool IsBuildable(string word, string[] words)
    {
        for (var len = 1; len <= word.Length; len++)
        {
            var prefix = word[..len];
            var found = false;

            foreach (var candidate in words)
            {
                if (candidate == prefix)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own AlphabetTrie over the lowercase letters: build one trie from the whole word list,
    // then walk it from the root descending only through nodes whose HasValue is
    // already true - a step is legal exactly when the prefix reached so far is
    // itself a complete dictionary word, which is this problem's own "built one
    // character at a time" rule. The walk visits only the letters each node has a child
    // for, in letter order, and the packed layout: once no node is asked for an absent
    // letter, packed nodes measured faster than dense ones, with a third of the allocation.
    public static string LongestWordByLowercaseTrieWalk(string[] words)
    {
        var trie = BuildTrie(words);

        return FindLongestWord(trie.Root, new CandidateWord(LongestWordInDictionaryConstants.EmptyPrefix), new BestWord(NoQualifyingWord)).Text;
    }

    private static WordTrie BuildTrie(string[] words)
    {
        var trie = new WordTrie();

        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        return trie;
    }

    private static bool IsBetter(CandidateWord candidate, BestWord best) =>
        candidate.Text.Length > best.Text.Length
        || (candidate.Text.Length == best.Text.Length && string.CompareOrdinal(candidate.Text, best.Text) < 0);

    private static BestWord FindLongestWord(WordNode node, CandidateWord prefix, BestWord best)
    {
        best = BetterOf(prefix, best);

        foreach (var (letter, child) in node.Children)
        {
            best = VisitChild(child, letter, prefix, best);
        }

        return best;
    }

    private static BestWord VisitChild(WordNode child, int letter, CandidateWord prefix, BestWord best)
    {
        if (!child.HasValue)
        {
            return best;
        }

        return FindLongestWord(child, new CandidateWord(prefix.Text + EnglishLowercase.LetterAt(letter)), best);
    }

    private static BestWord BetterOf(CandidateWord prefix, BestWord best)
    {
        if (prefix.Text.Length == 0)
        {
            return best;
        }

        return IsBetter(prefix, best) ? new BestWord(prefix.Text) : best;
    }

    // The two things being weighed against each other at every step of both strategies: the
    // word (or trie prefix) currently on offer, and the best complete word accepted so far.
    // Named, rather than left as two interchangeable `string` positions a caller could hand
    // over the wrong way round with the compiler none the wiser.
    private readonly record struct CandidateWord(string Text);

    private readonly record struct BestWord(string Text);
}
