using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MatchSubstringAfterReplacement;

// LeetCode 2301. Match Substring After Replacement: pattern matches at a start index
// in source when every position either equals source's character outright or is
// reachable from it via one of the allowed (old, new) replacement pairs.
// Replacements are optional and one-directional - a pair lets a character of pattern
// become the paired character, never the reverse - and each character of pattern is
// replaced at most once, so the pairs never chain.
//
// Both strategies walk the same candidate start positions; they differ only in how
// "is (patternChar, sourceChar) an allowed pair" is answered.
internal static class MatchSubstringAfterReplacementSolution
{
    // The textbook baseline this composition has to justify itself against: keep the
    // pairs as the flat list they arrive in and scan it on every mismatched
    // character. Deliberately written with nothing but the input array - it is what
    // you would write without this repo, and its cost is one full pass over the
    // mappings per character comparison.
    public static bool IsMatchByLinearScan(
        SourceText source, SubstringPattern pattern, (char Old, char New)[] mappings)
    {
        for (var start = 0; start + pattern.Text.Length <= source.Text.Length; start++)
        {
            if (IsMatchAtByLinearScan(source, pattern, start, mappings))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsMatchAtByLinearScan(
        SourceText source, SubstringPattern pattern, int start, (char Old, char New)[] mappings)
    {
        for (var j = 0; j < pattern.Text.Length; j++)
        {
            var patternChar = pattern.Text[j];
            var sourceChar = source.Text[start + j];

            if (patternChar != sourceChar && !HasMapping(mappings, patternChar, sourceChar))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasMapping((char Old, char New)[] mappings, char patternChar, char sourceChar)
    {
        foreach (var (oldChar, newChar) in mappings)
        {
            if (oldChar == patternChar && newChar == sourceChar)
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
    public static bool IsMatchByHashMapLookup(
        SourceText source, SubstringPattern pattern, (char Old, char New)[] mappings)
    {
        var allowed = BuildAllowedMap(mappings);

        for (var start = 0; start + pattern.Text.Length <= source.Text.Length; start++)
        {
            if (IsMatchAtByHashMapLookup(source, pattern, start, allowed))
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

    private static bool IsMatchAtByHashMapLookup(
        SourceText source, SubstringPattern pattern, int start, HashMap<char, Set<char>> allowed)
    {
        for (var j = 0; j < pattern.Text.Length; j++)
        {
            var patternChar = pattern.Text[j];
            var sourceChar = source.Text[start + j];

            if (patternChar == sourceChar)
            {
                continue;
            }

            if (!allowed.TryGetValue(patternChar, out var targets) || !targets.Has(sourceChar))
            {
                return false;
            }
        }

        return true;
    }

    // The two ends of LC 2301's candidate window, named for the roles they play here
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser. `source` is the text whose
    // every window is tried; `pattern` is the pattern that has to match one of them.
    // The two are not interchangeable - swapping them asks whether `source` occurs
    // inside `pattern` - so a transposition is a silently wrong answer rather than a
    // different question.
    internal readonly record struct SourceText(string Text);

    internal readonly record struct SubstringPattern(string Text);
}
