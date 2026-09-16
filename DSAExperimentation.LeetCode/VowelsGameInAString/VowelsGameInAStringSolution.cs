using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.VowelsGameInAString;

// LeetCode 3227. Vowels Game in a String: Alice must remove a substring with an
// odd vowel count on her turn, Bob a substring with an even one (0 included) on
// his; whoever cannot move loses.
//
// The two turn kinds are not interchangeable - a state that is winning for
// "whoever moves next" in most turn games instead has to be tracked once per
// role here - but the game still collapses to one fact: Alice wins iff `text`
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
    public static bool CanAliceWinByGameSearch(string text)
    {
        var aliceMemo = new Dictionary<string, bool>();
        var bobMemo = new Dictionary<string, bool>();
        return IsWinningForAliceToMove(text, aliceMemo, bobMemo);
    }

    // The derived closed form: Alice wins iff `text` has at least one vowel.
    public static bool CanAliceWinByVowelExistence(string text)
    {
        foreach (var c in text)
        {
            if (VowelSet.Has(c))
            {
                return true;
            }
        }

        return false;
    }

    // Alice needs a substring with an odd vowel count; she wins if any such
    // removal leaves Bob facing a state where he cannot win.
    private static bool IsWinningForAliceToMove(
        string text, Dictionary<string, bool> aliceMemo, Dictionary<string, bool> bobMemo)
    {
        if (aliceMemo.TryGetValue(text, out var cached))
        {
            return cached;
        }

        var wins = HasWinningRemoval(text, VowelParity.Odd, (aliceMemo, bobMemo));
        aliceMemo[text] = wins;
        return wins;
    }

    // Bob needs a substring with an even (0 included) vowel count; he wins if
    // any such removal leaves Alice facing a state where she cannot win.
    private static bool IsWinningForBobToMove(
        string text, Dictionary<string, bool> aliceMemo, Dictionary<string, bool> bobMemo)
    {
        if (bobMemo.TryGetValue(text, out var cached))
        {
            return cached;
        }

        var wins = HasWinningRemoval(text, VowelParity.Even, (aliceMemo, bobMemo));
        bobMemo[text] = wins;
        return wins;
    }

    // Tries every substring [start, end] in order, checking each one's vowel
    // count against the mover's required parity, and stops at the first
    // removal that makes the opponent lose.
    private static bool HasWinningRemoval(
        string text,
        VowelParity requiredParity,
        (Dictionary<string, bool> AliceMemo, Dictionary<string, bool> BobMemo) memos)
    {
        for (var start = 0; start < text.Length; start++)
        {
            if (HasWinningRemovalFrom(text, start, requiredParity, memos))
            {
                return true;
            }
        }

        return false;
    }

    // Grows the removal's right end from `start`, counting the vowels it takes in,
    // and reports whether any prefix with this start has the required parity and
    // leaves the opponent losing.
    private static bool HasWinningRemovalFrom(
        string text,
        int start,
        VowelParity requiredParity,
        (Dictionary<string, bool> AliceMemo, Dictionary<string, bool> BobMemo) memos)
    {
        var vowels = 0;

        for (var end = start; end < text.Length; end++)
        {
            if (Vowels.Contains(text[end]))
            {
                vowels++;
            }

            if (!HasVowelParity(vowels, requiredParity))
            {
                continue;
            }

            var rest = text.Remove(start, end - start + 1);
            if (IsLosingForOpponent(rest, requiredParity, memos))
            {
                return true;
            }
        }

        return false;
    }

    // The question a removal asks of the position it leaves behind: does whoever
    // moves next lose from here? This mover's own parity fixes which of the two
    // turn functions answers it, since only the other role can be next to move.
    private static bool IsLosingForOpponent(
        string remaining,
        VowelParity movedParity,
        (Dictionary<string, bool> AliceMemo, Dictionary<string, bool> BobMemo) memos)
    {
        if (movedParity == VowelParity.Odd)
        {
            return !IsWinningForBobToMove(remaining, memos.AliceMemo, memos.BobMemo);
        }

        return !IsWinningForAliceToMove(remaining, memos.AliceMemo, memos.BobMemo);
    }

    // A removal is legal only when its vowel count has the parity its mover needs.
    private static bool HasVowelParity(int vowels, VowelParity requiredParity) =>
        (vowels % 2 == 1) == (requiredParity == VowelParity.Odd);

    // Which vowel-count parity a legal removal must have: Alice needs an odd one,
    // Bob an even one (zero included).
    private enum VowelParity
    {
        Even,
        Odd,
    }
}
