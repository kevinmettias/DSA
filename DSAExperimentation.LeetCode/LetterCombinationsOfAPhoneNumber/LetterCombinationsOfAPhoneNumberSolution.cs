using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.LetterCombinationsOfAPhoneNumber;

// LeetCode 17. Letter Combinations of a Phone Number: each digit maps to its
// phone-keypad letters, and the answer is every string formed by choosing one
// letter per digit in order - the Cartesian product of the digits' letter sets,
// in phone-keypad order.
//
// The two strategies differ only in how they build that product: nested list
// expansion carried by hand, or this repo's generic Backtrack.Search walking one
// decision level per digit position.
internal static class LetterCombinationsOfAPhoneNumberSolution
{
    // Phone-keypad letter groups, keyed by digit (LC 17 mapping).
    private const string Digit2Letters = "abc";
    private const string Digit3Letters = "def";
    private const string Digit4Letters = "ghi";
    private const string Digit5Letters = "jkl";
    private const string Digit6Letters = "mno";
    private const string Digit7Letters = "pqrs";
    private const string Digit8Letters = "tuv";
    private const string Digit9Letters = "wxyz";

    // The textbook answer: grow the result list one digit at a time, appending
    // every letter of the next digit to every combination built so far.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed backtracking search below has to justify itself against.
    public static List<string> LetterCombinationsByIterativeExpansion(string digits)
    {
        if (digits.Length == 0)
        {
            return [];
        }

        var results = new List<string> { string.Empty };

        foreach (var digit in digits)
        {
            var next = new List<string>();

            foreach (var prefix in results)
            {
                foreach (var letter in LettersFor(digit))
                {
                    next.Add(prefix + letter);
                }
            }

            results = next;
        }

        return results;
    }

    // This repo's own Backtrack.Search: each digit position is a decision level,
    // Candidates offers that digit's letters, and a choice sequence as long as the
    // input is a solution.
    public static List<string> LetterCombinationsByBacktracking(string digits)
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
            '2' => Digit2Letters,
            '3' => Digit3Letters,
            '4' => Digit4Letters,
            '5' => Digit5Letters,
            '6' => Digit6Letters,
            '7' => Digit7Letters,
            '8' => Digit8Letters,
            '9' => Digit9Letters,
            _ => string.Empty,
        };

    private sealed class CombinationState
    {
        public List<char> Chosen { get; } = [];

        public int Index { get; set; }
    }
}
