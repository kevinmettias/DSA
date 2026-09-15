using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MatchSubstringAfterReplacement;

// LeetCode 2301. Match Substring After Replacement: sub matches at a start index in
// s when every position either equals s's character outright or is reachable from
// it via one of the allowed (old, new) replacement pairs. Replacements are optional
// and one-directional - a pair lets a character of sub become the paired character,
// never the reverse - and each character of sub is replaced at most once, so the
// pairs never chain.
//
// Both strategies walk the same candidate start positions; they differ only in how
// "is (subChar, sChar) an allowed pair" is answered.
internal static class MatchSubstringAfterReplacementSolution
{
    // The textbook baseline this composition has to justify itself against: keep the
    // pairs as the flat list they arrive in and scan it on every mismatched
    // character. Deliberately written with nothing but the input array - it is what
    // you would write without this repo, and its cost is one full pass over the
    // mappings per character comparison.
    public static bool IsMatchByLinearScan(SourceText s, SubstringPattern sub, (char Old, char New)[] mappings)
    {
        for (var start = 0; start + sub.Text.Length <= s.Text.Length; start++)
        {
            if (MatchesAtByLinearScan(s, sub, start, mappings))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAtByLinearScan(
        SourceText s, SubstringPattern sub, int start, (char Old, char New)[] mappings)
    {
        for (var j = 0; j < sub.Text.Length; j++)
        {
            var subChar = sub.Text[j];
            var sChar = s.Text[start + j];

            if (subChar != sChar && !HasMapping(mappings, subChar, sChar))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasMapping((char Old, char New)[] mappings, char subChar, char sChar)
    {
        foreach (var (oldChar, newChar) in mappings)
        {
            if (oldChar == subChar && newChar == sChar)
            {
                return true;
            }
        }

        return false;
    }

    // The composed answer: index the pairs once as this repo's own
    // HashMap<char, Set<char>> - keyed by the "old" character, each value a
    // Set<Element> (itself HashMap<Element, bool>, ARCHITECTURE.md §4.1) of the
    // characters that old is allowed to become - turning the baseline's scan into an
    // O(1) two-step lookup. Building the index is charged to this method, since it
    // is part of what the composition has to pay for.
    public static bool IsMatchByHashMapLookup(SourceText s, SubstringPattern sub, (char Old, char New)[] mappings)
    {
        var allowed = BuildAllowedMap(mappings);

        for (var start = 0; start + sub.Text.Length <= s.Text.Length; start++)
        {
            if (MatchesAtByHashMapLookup(s, sub, start, allowed))
            {
                return true;
            }
        }

        return false;
    }

    private static HashMap<char, Set<char>> BuildAllowedMap((char Old, char New)[] mappings)
    {
        var allowed = new HashMap<char, Set<char>>();

        foreach (var (oldChar, newChar) in mappings)
        {
            if (!allowed.TryGetValue(oldChar, out var targets))
            {
                targets = new Set<char>();
                allowed.Set(oldChar, targets);
            }

            targets.TryAdd(newChar);
        }

        return allowed;
    }

    private static bool MatchesAtByHashMapLookup(
        SourceText s, SubstringPattern sub, int start, HashMap<char, Set<char>> allowed)
    {
        for (var j = 0; j < sub.Text.Length; j++)
        {
            var subChar = sub.Text[j];
            var sChar = s.Text[start + j];

            if (subChar == sChar)
            {
                continue;
            }

            if (!allowed.TryGetValue(subChar, out var targets) || !targets.Has(sChar))
            {
                return false;
            }
        }

        return true;
    }

    // The two ends of LC 2301's candidate window, named for the roles they play here
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser. `s` is the text whose every
    // window is tried; `sub` is the pattern that has to match one of them. The two are
    // not interchangeable - swapping them asks whether `s` occurs inside `sub` - so a
    // transposition is a silently wrong answer rather than a different question.
    internal readonly record struct SourceText(string Text);

    internal readonly record struct SubstringPattern(string Text);
}
