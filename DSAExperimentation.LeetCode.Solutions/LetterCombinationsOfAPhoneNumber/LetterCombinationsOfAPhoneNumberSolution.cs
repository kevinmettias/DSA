using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Domain.Devices.Telephone;

namespace DSAExperimentation.LeetCode.Solutions.LetterCombinationsOfAPhoneNumber;

// LeetCode 17. Letter Combinations of a Phone Number: each digit maps to its
// phone-keypad letters, and the answer is every string formed by choosing one
// letter per digit in order - the Cartesian product of the digits' letter sets,
// in phone-keypad order.
//
// The two strategies differ only in how they build that product: nested list
// expansion carried by hand, or this repo's generic Backtrack.Search walking one
// decision level per digit position.
//
// The keypad is the telephone's (Domain/Devices/Telephone): both arms read a digit's letters from
// Keypad<E161Keypad> (ARCHITECTURE §17.5).
[LeetCodeQuestion("letter-combinations-of-a-phone-number", LeetCodeAnswerOrder.AnyOrder)]
internal static class LetterCombinationsOfAPhoneNumberSolution
{
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
            results = ExpandWithDigit(results, digit);
        }

        return results;
    }

    // One round of the product: every letter of this digit appended to every
    // prefix built so far, in phone-keypad order.
    private static List<string> ExpandWithDigit(List<string> prefixes, char digit)
    {
        var next = new List<string>();

        foreach (var prefix in prefixes)
        {
            foreach (var letter in LettersFor(digit))
            {
                next.Add(prefix + letter);
            }
        }

        return next;
    }

    private static string LettersFor(char digit) => Keypad<E161Keypad>.KeyFor(digit).Spelling;

    // This repo's own Backtrack.Search: each digit position is a decision level,
    // Candidates offers that digit's letters, and a choice sequence as long as the
    // input is a solution.
    public static List<string> LetterCombinationsByBacktracking(string digits)
    {
        if (digits.Length == 0)
        {
            return [];
        }

        return SearchCombinations(digits);
    }

    private static List<string> SearchCombinations(string digits)
    {
        var results = new List<string>();
        var state = new CombinationState();

        Backtrack.Search<CombinationState, char>(
            state,
            isSolution: s => s.Index == digits.Length,
            candidates: s => s.Index == digits.Length ? Array.Empty<char>() : Keypad<E161Keypad>.KeyFor(digits[s.Index]).Spelling,
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

    private sealed class CombinationState
    {
        public List<char> Chosen { get; } = [];

        public int Index { get; set; }
    }
}
