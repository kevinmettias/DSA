using DSAExperimentation.LeetCode.WordsWithinTwoEditsOfDictionary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordsWithinTwoEditsOfDictionary;

// Harness only: both strategies live in WordsWithinTwoEditsOfDictionarySolution and
// are asserted against the same examples, including the budget boundary (two edits
// match, three do not), a query that matches nothing, and duplicate queries, which
// LeetCode reports once each rather than collapsing.
public sealed class WordsWithinTwoEditsOfDictionaryTests
{
    public static TheoryData<string[], string[], string[]> Examples =>
        new()
        {
            // LeetCode example 1: "ants" is three edits from every dictionary word.
            { ["word", "note", "ants", "wood"], ["word", "note", "cash"], ["word", "note", "wood"] },

            // LeetCode example 2: nothing is within budget.
            { ["yes"], ["not"], [] },

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
    [MemberData(nameof(Examples))]
    public void FindMatchingQueriesByBruteForce_LeetCodeExamples_ReturnsQueriesWithinEditBudget(
        string[] queries, string[] dictionary, string[] expected)
    {
        var actual = WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByBruteForce(
            queries, dictionary);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMatchingQueriesByEditBudgetTrie_LeetCodeExamples_ReturnsQueriesWithinEditBudget(
        string[] queries, string[] dictionary, string[] expected)
    {
        var actual = WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByEditBudgetTrie(
            queries, dictionary);

        Assert.Equal(expected, actual);
    }
}
