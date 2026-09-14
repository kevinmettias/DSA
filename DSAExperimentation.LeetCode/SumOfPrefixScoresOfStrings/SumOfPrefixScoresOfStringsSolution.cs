using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SumOfPrefixScoresOfStrings;

// LeetCode 2416. Sum of Prefix Scores of Strings: a string's score is the number of
// words in the list that have it as a prefix, and answer[i] is the sum of the scores
// of every non-empty prefix of words[i] - including words[i] itself.
//
// Both strategies answer that same question; they differ only in whether the "how
// many words start with this prefix" count is recomputed by re-scanning the word
// list, or read off a trie that counted it once while the words were inserted.
internal static class SumOfPrefixScoresOfStringsSolution
{
    // The textbook answer: for each word, take each of its prefixes in turn and
    // count the words that start with it, with string.StartsWith and nothing else.
    // O(wordCount^2 * wordLength). Deliberately without this repo's primitives
    // (section 17.5) - it is the arm the trie below has to justify itself against.
    public static int[] SumPrefixScoresByStartsWithScan(string[] words)
    {
        var scores = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            scores[i] = PrefixMatchCount(words, words[i]);
        }

        return scores;
    }

    private static int PrefixMatchCount(string[] words, string word)
    {
        var score = 0;

        for (var length = 1; length <= word.Length; length++)
        {
            var prefix = word[..length];

            foreach (var candidate in words)
            {
                if (candidate.StartsWith(prefix, StringComparison.Ordinal))
                {
                    score++;
                }
            }
        }

        return score;
    }

    // This repo's own LowercaseTrie<int>, the bounded-alphabet trie MapSumPairs and
    // CountingWordsWithAGivenPrefix already use. Instead of storing a value only at
    // each word's end node the way Set does, every word is walked in through the
    // already-public Root/Children and every node on its path has its Value
    // incremented - so a node's Value ends up being exactly the score of the prefix
    // that reaches it. Scoring a word is then one more walk, summing the counts
    // already sitting along its own root-to-leaf path: O(wordCount * wordLength) to
    // build plus O(wordCount * wordLength) to score.
    public static int[] SumPrefixScoresByPrefixCountingTrie(string[] words)
    {
        var trie = new LowercaseTrie<int>();

        foreach (var word in words)
        {
            CountPrefixesOf(trie.Root, word);
        }

        var scores = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            scores[i] = ScoreAlong(trie.Root, words[i]);
        }

        return scores;
    }

    private static void CountPrefixesOf(LowercaseTrieNode<int> root, string word)
    {
        var current = root;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a'] ??= new LowercaseTrieNode<int>();
            current.Value++;
        }
    }

    private static int ScoreAlong(LowercaseTrieNode<int> root, string word)
    {
        var current = root;
        var score = 0;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a']!;
            score += current.Value;
        }

        return score;
    }
}
