using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.VowelsGameInAString;

// LeetCode 3227. Vowels Game in a String: Alice must remove a substring with an
// odd vowel count on her turn, Bob a substring with an even one (0 included) on
// his; whoever cannot move loses.
//
// The two turn kinds are not interchangeable - a state that is winning for
// "whoever moves next" in most turn games instead has to be tracked once per
// role here - but the game still collapses to one fact: Alice wins iff s
// contains at least one vowel. If the total vowel count is odd she can clear
// the whole string in one move and win immediately; if it is even but
// positive she removes a single vowel, which leaves an odd count for Bob and
// forces the same trap on him one level down (his even-only moves can never
// produce a losing-for-Alice odd-zero split), all the way down to a lone vowel
// he cannot legally touch. With zero vowels she has no legal first move.
internal static class VowelsGameInAStringSolution
{
    private const string Vowels = "aeiou";

    private static readonly Set<char> VowelSet = new(Vowels);

    // The textbook answer: play out the actual game tree. Alice's and Bob's
    // turns need separate recursive functions - the substrings each may
    // legally remove differ by role, not by whose turn it happens to be - each
    // memoized on the exact remaining string, since splicing the two surviving
    // ends back together after a removal can make different removal choices
    // land on the same string. Deliberately written without this repo's
    // primitives beyond a plain Dictionary, the arm the closed form below has
    // to agree with.
    public static bool DoesAliceWinByGameSearch(string s)
    {
        var aliceMemo = new Dictionary<string, bool>();
        var bobMemo = new Dictionary<string, bool>();
        return AliceToMoveWins(s, aliceMemo, bobMemo);
    }

    // Alice needs a substring with an odd vowel count; she wins if any such
    // removal leaves Bob facing a state where he cannot win.
    private static bool AliceToMoveWins(
        string s, Dictionary<string, bool> aliceMemo, Dictionary<string, bool> bobMemo)
    {
        if (aliceMemo.TryGetValue(s, out var cached))
        {
            return cached;
        }

        var wins = HasWinningRemoval(s, requireOddVowels: true, rest => !BobToMoveWins(rest, aliceMemo, bobMemo));
        aliceMemo[s] = wins;
        return wins;
    }

    // Bob needs a substring with an even (0 included) vowel count; he wins if
    // any such removal leaves Alice facing a state where she cannot win.
    private static bool BobToMoveWins(
        string s, Dictionary<string, bool> aliceMemo, Dictionary<string, bool> bobMemo)
    {
        if (bobMemo.TryGetValue(s, out var cached))
        {
            return cached;
        }

        var wins = HasWinningRemoval(s, requireOddVowels: false, rest => !AliceToMoveWins(rest, aliceMemo, bobMemo));
        bobMemo[s] = wins;
        return wins;
    }

    // Tries every substring [start, end] in order, checking each one's vowel
    // count against the mover's required parity, and stops at the first
    // removal that makes the opponent lose.
    private static bool HasWinningRemoval(string s, bool requireOddVowels, Func<string, bool> opponentLoses)
    {
        for (var start = 0; start < s.Length; start++)
        {
            var vowels = 0;

            for (var end = start; end < s.Length; end++)
            {
                if (Vowels.Contains(s[end]))
                {
                    vowels++;
                }

                if (vowels % 2 == 1 != requireOddVowels)
                {
                    continue;
                }

                var rest = s.Remove(start, end - start + 1);
                if (opponentLoses(rest))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // The derived closed form: Alice wins iff s has at least one vowel.
    public static bool DoesAliceWinByVowelExistence(string s)
    {
        foreach (var c in s)
        {
            if (VowelSet.Has(c))
            {
                return true;
            }
        }

        return false;
    }
}
