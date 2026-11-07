using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.MaximumScoreWordsFormedByLetters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumScoreWordsFormedByLetters;

// Harness only: both strategies live in MaximumScoreWordsFormedByLettersSolution and
// are asserted against the same cases - including one where taking the locally
// obvious word is wrong, which is what makes the backtracking necessary. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MaximumScoreWordsFormedByLettersSolutionTests
{
    public static TheoryData<string[], char[], int[], int> Cases =>
        new()
        {
            // Example 1's words and letters with 'o' scored 0 rather than LeetCode's 2.
            // Available letters give a:2, c:1, d:3, g:1, o:2 and no 't' at all, so
            // "cat" can never be formed - only "dog" (5+0+3=8), "dad" (5+1+5=11) and
            // "good" (3+0+0+5=8) are ever reachable, and the d-budget (3) rules out
            // taking all three together (1+2+1=4 d's needed). The best reachable
            // pair is "dad"+"good" (equivalently "dog"+"dad"), both summing to 19.
            {
                ["dog", "cat", "dad", "good"],
                ['a', 'a', 'c', 'd', 'd', 'd', 'g', 'o', 'o'],
                LetterScores(('a', 1), ('c', 9), ('d', 5), ('g', 3)),
                19
            },

            // Letters exactly cover every word, so every word is taken.
            {
                ["cat", "dog"],
                ['c', 'a', 't', 'd', 'o', 'g'],
                LetterScores(('c', 1), ('a', 1), ('t', 1), ('d', 1), ('o', 1), ('g', 1)),
                6
            },

            // Nothing is formable: the pool has no 'b'.
            {
                ["ab"],
                ['a'],
                LetterScores(('a', 1), ('b', 2)),
                0
            },

            // Skipping the first word is the winning move: "aab" (1+1+10=12) and
            // "bb" (20) both fit individually but need three b's together, and only
            // two are available.
            {
                ["aab", "bb"],
                ['a', 'a', 'b', 'b'],
                LetterScores(('a', 1), ('b', 10)),
                20
            },

            // Two identical words competing for one shared pool: three a's spell
            // "aa" once, not twice.
            {
                ["aa", "aa"],
                ['a', 'a', 'a'],
                LetterScores(('a', 1)),
                2
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreWordsByNaiveRecursion_Cases_ReturnsBestReachableSubsetScore(
        string[] words,
        char[] letters,
        int[] score,
        int expected)
    {
        var actual = MaximumScoreWordsFormedByLettersSolution.MaxScoreWordsByNaiveRecursion(words, letters, score);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreWordsByBacktrackSearch_Cases_ReturnsBestReachableSubsetScore(
        string[] words,
        char[] letters,
        int[] score,
        int expected)
    {
        var actual = MaximumScoreWordsFormedByLettersSolution.MaxScoreWordsByBacktrackSearch(words, letters, score);

        Assert.Equal(expected, actual);
    }

    // LeetCode states score as a dense 26-entry vector; naming only the non-zero
    // letters keeps each case above readable.
    private static int[] LetterScores(params (char Letter, int Value)[] scores)
    {
        var score = new int[EnglishLetters.Size];

        foreach (var (letter, value) in scores)
        {
            score[letter - 'a'] = value;
        }

        return score;
    }
}
