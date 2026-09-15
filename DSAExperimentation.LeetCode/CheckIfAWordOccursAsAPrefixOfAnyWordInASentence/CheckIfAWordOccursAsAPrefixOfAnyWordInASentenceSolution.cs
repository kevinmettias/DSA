using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

// LeetCode 1455. Check If a Word Occurs As a Prefix of Any Word in a Sentence:
// report the 1-indexed position of the first space-separated word of `sentence`
// that starts with `searchWord`, or -1 when no word does.
//
// Both strategies walk the words left to right and stop at the first match, so
// they only differ in how "does this word start with searchWord" is decided: a
// direct character comparison, or an insert-then-HasPrefix round trip through
// this repo's own Trie<TValue>.
internal static class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution
{
    private const char WordSeparator = ' ';

    // The textbook answer: split on spaces and ask string.StartsWith. Deliberately
    // written with nothing but the BCL - it is the arm the Trie composition below
    // has to justify itself against.
    public static int IndexOfPrefixWordByStartsWithScan(SentenceText sentence, SearchedPrefix searchWord) =>
        IndexOfPrefixWordByStartsWithScan(SplitWords(sentence.Text), searchWord.Text);

    public static int IndexOfPrefixWordByStartsWithScan(DynamicArray<string> words, string searchWord)
    {
        for (var i = 0; i < words.Count; i++)
        {
            if (words.Get(i).StartsWith(searchWord, StringComparison.Ordinal))
            {
                return i + 1;
            }
        }

        return LeetCodeAnswer.None;
    }

    // This repo's own Trie: insert one candidate word, then ask HasPrefix - exactly
    // ImplementTrie's "insert 'apple', then HasPrefix('app') is true" pairing
    // (LC 208), re-run per word until the first match. A fresh Trie per word is what
    // keeps the question "does THIS word start with searchWord" rather than "does
    // any word so far".
    public static int IndexOfPrefixWordByTriePerWord(SentenceText sentence, SearchedPrefix searchWord) =>
        IndexOfPrefixWordByTriePerWord(SplitWords(sentence.Text), searchWord.Text);

    public static int IndexOfPrefixWordByTriePerWord(DynamicArray<string> words, string searchWord)
    {
        for (var i = 0; i < words.Count; i++)
        {
            var trie = new Trie<bool>();
            trie.Set(words.Get(i), true);

            if (trie.HasPrefix(searchWord))
            {
                return i + 1;
            }
        }

        return LeetCodeAnswer.None;
    }

    // LeetCode hands the sentence as one string; splitting it is input shaping, not
    // part of either strategy, so the hoisted overloads above take the split form
    // and a benchmark can charge this to [GlobalSetup].
    public static DynamicArray<string> SplitWords(string sentence)
    {
        var words = new DynamicArray<string>();

        foreach (var word in sentence.Split(WordSeparator))
        {
            words.Add(word);
        }

        return words;
    }

    // LC 1455's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `sentence` is the whole space-separated sentence, and
    // `searchWord` the prefix looked for at the start of one of its words - asking
    // whether the search word occurs in the sentence is not the reverse question.
    // The hoisted overloads above take the already-split form, so only the
    // whole-sentence entry points carry the pair.
    internal readonly record struct SentenceText(string Text);

    internal readonly record struct SearchedPrefix(string Text);
}
