using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LetterCombinationsOfAPhoneNumber;

// LeetCode 17. Letter Combinations of a Phone Number: each digit position is a
// backtracking decision level, so Backtrack.Search enumerates the product space.
public sealed partial class LetterCombinationsOfAPhoneNumberTests
{
    [Fact]
    public void LetterCombinations_TwoDigits_ReturnsCartesianProductInPhoneOrder()
    {
        var combinations = LetterCombinations("23");

        Assert.Equal(["ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf"], combinations);
    }

    [Fact]
    public void LetterCombinations_EmptyInput_ReturnsEmptyList()
        => Assert.Empty(LetterCombinations(""));

    private static List<string> LetterCombinations(string digits)
    {
        if (digits.Length == 0)
        {
            return [];
        }

        var results = new List<string>();
        var state = new CombinationState();

        Backtrack.Search<CombinationState, char>(
            state,
            isSolution: s => s.Index == digits.Length,
            candidates: s => s.Index == digits.Length ? [] : LettersFor(digits[s.Index]),
            choose: (s, letter) =>
            {
                s.Chosen.Add(letter);
                s.Index++;
            },
            unchoose: (s, _) =>
            {
                s.Index--;
                s.Chosen.RemoveAt(s.Chosen.Count - 1);
            },
            onSolution: s => results.Add(new string(s.Chosen.ToArray())));

        return results;
    }

    private static string LettersFor(char digit)
        => digit switch
        {
            '2' => "abc",
            '3' => "def",
            '4' => "ghi",
            '5' => "jkl",
            '6' => "mno",
            '7' => "pqrs",
            '8' => "tuv",
            '9' => "wxyz",
            _ => string.Empty,
        };

    private sealed class CombinationState
    {
        public List<char> Chosen { get; } = [];

        public int Index { get; set; }
    }
}

