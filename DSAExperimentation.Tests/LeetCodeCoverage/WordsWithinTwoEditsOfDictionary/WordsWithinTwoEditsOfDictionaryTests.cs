using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordsWithinTwoEditsOfDictionary;

// LeetCode 2452. Words Within Two Edits of Dictionary: build every dictionary word
// into this repo's own LowercaseTrie<bool> (all words are guaranteed lowercase and
// equal length), then, per query, a budgeted DFS over the trie - the same
// Root/Children navigation ImplementMagicDictionaryTests.Search already rehearses -
// generalized from "exactly one substitution" to "at most two," succeeding as soon
// as the walk consumes the whole query with budget left and lands on a stored word.
public sealed class WordsWithinTwoEditsOfDictionaryTests
{
    private const int MaxEdits = 2;

    [Fact]
    public void FindMatchingQueries_LeetCodeExample1_ReturnsWordsWithinBudget()
    {
        string[] queries = ["word", "note", "ants", "wood"];
        string[] dictionary = ["word", "note", "cash"];

        var result = FindMatchingQueries(queries, dictionary);

        Assert.Equal(["word", "note", "wood"], result);
    }

    [Fact]
    public void FindMatchingQueries_LeetCodeExample2_ReturnsNoMatches()
    {
        string[] queries = ["yes"];
        string[] dictionary = ["not"];

        var result = FindMatchingQueries(queries, dictionary);

        Assert.Empty(result);
    }

    [Fact]
    public void FindMatchingQueries_ExactDictionaryWord_CountsAsZeroEditsWithinBudget()
    {
        string[] queries = ["hello"];
        string[] dictionary = ["hello"];

        var result = FindMatchingQueries(queries, dictionary);

        Assert.Equal(["hello"], result);
    }

    private static string[] FindMatchingQueries(string[] queries, string[] dictionary)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in dictionary)
        {
            trie.Set(word, true);
        }

        var matches = new List<string>();

        foreach (var query in queries)
        {
            if (IsWithinEditBudget(trie.Root, query, 0, MaxEdits))
            {
                matches.Add(query);
            }
        }

        return [.. matches];
    }

    private static bool IsWithinEditBudget(LowercaseTrieNode<bool> node, string query, int index, int remainingEdits)
    {
        if (index == query.Length)
        {
            return node.HasValue;
        }

        var target = query[index] - 'a';

        for (var candidate = 0; candidate < LowercaseTrieNode<bool>.AlphabetSize; candidate++)
        {
            var next = node.Children[candidate];

            if (next is null)
            {
                continue;
            }

            var nextBudget = candidate == target ? remainingEdits : remainingEdits - 1;

            if (nextBudget >= 0 && IsWithinEditBudget(next, query, index + 1, nextBudget))
            {
                return true;
            }
        }

        return false;
    }
}
