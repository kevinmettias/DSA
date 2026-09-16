using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestNiceSubstring;

// LeetCode 1763. Longest Nice Substring: the longest substring in which every
// letter that appears does so in both cases. Ties go to the earliest such
// substring, and a string with no nice substring answers with the empty string.
//
// The two strategies differ in what they do with the observation that a single
// character missing its opposite-case partner poisons every substring spanning
// it. The baseline never makes that observation at all and tests each of the
// O(n^2) substrings independently; the divide-and-conquer strategy uses it to
// cut the string at the first such character and recurse on the two halves.
internal static class LongestNiceSubstringSolution
{
    // A single character can never be nice - it has no opposite-case partner
    // beside it - so anything shorter than this is answered with the empty string.
    private const int MinimumNiceSubstringLength = 2;

    // The presence table the baseline rebuilds per substring, wide enough for the
    // ASCII letters LC 1763's inputs are restricted to.
    private const int AsciiTableSize = 128;

    // The textbook answer: enumerate every substring longest-useful-first by
    // tracking the best length seen, and re-derive "is this one nice" from
    // scratch each time by filling a fresh presence table and rescanning it.
    // O(n^3) overall. Deliberately written with a BCL bool[] and nothing from
    // this repo - it is the arm the divide-and-conquer strategy below has to
    // justify itself against.
    public static string FindLongestNiceSubstringByBruteForceSubstrings(string text)
    {
        var best = string.Empty;

        for (var start = 0; start < text.Length; start++)
        {
            best = LongestNiceRunFrom(text, start, best);
        }

        return best;
    }

    // Extends the window from one start position, keeping the incumbent unless a
    // strictly longer nice substring turns up - so ties keep the earlier find,
    // which is the answer LeetCode asks for.
    private static string LongestNiceRunFrom(string text, int start, string best)
    {
        for (var end = start; end < text.Length; end++)
        {
            var length = end - start + 1;

            if (length > best.Length && IsNice(text, start, end))
            {
                best = text.Substring(start, length);
            }
        }

        return best;
    }

    private static bool IsNice(string text, int start, int end)
    {
        var present = new bool[AsciiTableSize];

        for (var i = start; i <= end; i++)
        {
            present[text[i]] = true;
        }

        for (var i = start; i <= end; i++)
        {
            if (!present[Partner(text[i])])
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own Set<char> collects every character present in the
    // (sub)string, then one scan finds the first character whose opposite-case
    // partner is missing from it. If none is missing the whole string is already
    // nice. Otherwise no nice substring can ever cross that character - it can
    // never gain its missing partner - so the answer is the longer of the two
    // halves split around it, recursed independently. O(n^2) worst case.
    public static string FindLongestNiceSubstringByDivideAndConquer(string text)
    {
        if (text.Length < MinimumNiceSubstringLength)
        {
            return string.Empty;
        }

        var present = BuildCharacterSet(text);

        return SplitAtMissingPartner(text, present) ?? text;
    }

    private static Set<char> BuildCharacterSet(string text)
    {
        var present = new Set<char>();

        foreach (var character in text)
        {
            present.TryAdd(character);
        }

        return present;
    }

    // Returns null when every character has its partner, i.e. text is already nice.
    private static string? SplitAtMissingPartner(string text, Set<char> present)
    {
        for (var i = 0; i < text.Length; i++)
        {
            if (!present.Has(Partner(text[i])))
            {
                return LongerHalf(text, i);
            }
        }

        return null;
    }

    // The left half wins ties, which is what makes the recursion report the
    // earliest longest nice substring rather than an arbitrary one.
    private static string LongerHalf(string text, int splitIndex)
    {
        var left = FindLongestNiceSubstringByDivideAndConquer(text[..splitIndex]);
        var right = FindLongestNiceSubstringByDivideAndConquer(text[(splitIndex + 1)..]);

        return left.Length >= right.Length ? left : right;
    }

    // The character that has to appear alongside `character` for `character` to be
    // satisfied.
    private static char Partner(char character) =>
        char.IsUpper(character) ? char.ToLower(character) : char.ToUpper(character);
}
