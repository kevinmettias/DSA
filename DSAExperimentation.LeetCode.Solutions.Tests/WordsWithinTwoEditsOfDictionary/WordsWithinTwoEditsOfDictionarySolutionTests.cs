using DSAExperimentation.LeetCode.Solutions.WordsWithinTwoEditsOfDictionary;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WordsWithinTwoEditsOfDictionary;

// Harness only: both strategies live in WordsWithinTwoEditsOfDictionarySolution and
// are asserted against the same cases, among them the budget boundary (two edits
// match, three do not), a query that matches nothing, and duplicate queries, which
// LeetCode reports once each rather than collapsing.
public sealed partial class WordsWithinTwoEditsOfDictionarySolutionTests
{
    public static TheoryData<string[], string[], string[]> Cases =>
        new()
        {
            // The queries of LeetCode's first example against a dictionary of this
            // file's own: "word" and "note" are in it, "wood" is one edit from "word",
            // and "ants" is at least three edits from every word.
            { ["word", "note", "ants", "wood"], ["word", "note", "cash"], ["word", "note", "wood"] },

            // An exact dictionary word costs zero edits, which is inside the budget.
            { ["hello"], ["hello"], ["hello"] },

            // The budget boundary walked one edit at a time: 0, 1 and 2 edits match,
            // 3 does not.
            { ["abcd", "abce", "abfe", "afge"], ["abcd"], ["abcd", "abce", "abfe"] },

            // Answers keep the queries' own order, so a later query can match while
            // an earlier one does not.
            { ["xyz", "abc", "xyc"], ["abc"], ["abc", "xyc"] },

            // Repeated queries are reported once each, not deduplicated.
            { ["ac", "ac"], ["ab"], ["ac", "ac"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMatchingQueriesByBruteForce_Cases_ReturnsQueriesWithinEditBudget(
        string[] queries, string[] dictionary, string[] expected)
    {
        var actual = WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByBruteForce(
            queries, dictionary);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMatchingQueriesByEditBudgetTrie_Cases_ReturnsQueriesWithinEditBudget(
        string[] queries, string[] dictionary, string[] expected)
    {
        var actual = WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByEditBudgetTrie(
            queries, dictionary);

        Assert.Equal(expected, actual);
    }
}
