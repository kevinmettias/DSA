using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.WordsWithinTwoEditsOfDictionary;

// LeetCode 2452. Words Within Two Edits of Dictionary: return every query that can
// be matched with some dictionary word after two or fewer edits, in the order the
// queries were given. Every word - query or dictionary entry - is lowercase and of
// the same length, so an "edit" is only ever a substitution at a fixed position;
// neither strategy has to consider insertions or deletions.
//
// Both strategies answer that same question and return the same list. They differ
// only in whether a query is compared against the dictionary one whole word at a
// time, or walked once against a trie in which the dictionary's shared prefixes
// have already been merged.
internal static class WordsWithinTwoEditsOfDictionarySolution
{
    // LeetCode's budget: a query matches a word it differs from in at most two positions.
    private const int MaxEdits = 2;

    // The textbook answer: for every query, scan the dictionary word by word and
    // count mismatched characters, stopping at the first word inside the budget.
    // O(queries * dictionarySize * wordLength). Deliberately written with nothing
    // but the BCL (section 17.5) - it is the arm the trie walk below has to justify
    // itself against.
    public static string[] FindMatchingQueriesByBruteForce(string[] queries, string[] dictionary)
    {
        var matches = new List<string>();

        foreach (var query in queries)
        {
            if (dictionary.Any(word => IsWithinEditBudget(word, query)))
            {
                matches.Add(query);
            }
        }

        return [.. matches];
    }

    private static bool IsWithinEditBudget(string word, string query)
    {
        var differences = 0;

        for (var i = 0; i < word.Length && differences <= MaxEdits; i++)
        {
            if (word[i] != query[i])
            {
                differences++;
            }
        }

        return differences <= MaxEdits;
    }

    // This repo's own LowercaseTrie<bool>, the bounded-alphabet trie that stores the
    // whole dictionary once with its common prefixes shared, walked per query by a
    // budgeted DFS over the already-public Root/Children - ImplementMagicDictionary's
    // one-substitution walk generalized from a budget of exactly 1 to "at most 2".
    // Descending into the child that matches the query's character is free; any other
    // child spends an edit, and the walk succeeds as soon as it consumes the whole
    // query with budget left over and lands on a stored word. O(dictionarySize *
    // wordLength) to build the trie once, plus one bounded walk per query.
    public static string[] FindMatchingQueriesByEditBudgetTrie(string[] queries, string[] dictionary)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in dictionary)
        {
            trie.Set(word, true);
        }

        var matches = new List<string>();

        foreach (var query in queries)
        {
            if (HasMatch(trie.Root, new SearchState(query, 0, MaxEdits)))
            {
                matches.Add(query);
            }
        }

        return [.. matches];
    }

    private readonly record struct SearchState(string Query, int Index, int RemainingEdits);

    private static bool HasMatch(LowercaseTrieNode<bool> node, SearchState state)
    {
        if (state.Index == state.Query.Length)
        {
            return node.HasValue;
        }

        return HasMatchInChildren(node, state);
    }

    private static bool HasMatchInChildren(LowercaseTrieNode<bool> node, SearchState state)
    {
        var target = state.Query[state.Index] - 'a';

        for (var candidate = 0; candidate < LowercaseAlphabet.Size; candidate++)
        {
            var next = node.Children[candidate];
            var match = candidate == target ? ChildMatch.Target : ChildMatch.Substitution;

            if (next is not null && TryMatchCandidate(next, state, match))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryMatchCandidate(LowercaseTrieNode<bool> next, SearchState state, ChildMatch match)
    {
        var nextBudget = match == ChildMatch.Target ? state.RemainingEdits : ReducedEditBudget(state);

        if (nextBudget < 0)
        {
            return false;
        }

        return HasMatch(next, state with { Index = state.Index + 1, RemainingEdits = nextBudget });
    }

    // A candidate that is not the target character spends one of the two edits.
    private static int ReducedEditBudget(SearchState state) => state.RemainingEdits - 1;

    // Which of the two kinds of descent into a candidate child this is: the character
    // the query wants next, or a substitution for it. Named so the call site says
    // which one it is instead of spelling it `true`.
    private enum ChildMatch
    {
        // The candidate is the query's own next character, so it costs no edit.
        Target,

        // The candidate is some other character, so it spends one of the two edits.
        Substitution,
    }
}
