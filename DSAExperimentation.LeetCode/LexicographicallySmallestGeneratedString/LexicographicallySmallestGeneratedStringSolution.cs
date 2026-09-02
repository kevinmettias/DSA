using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.LexicographicallySmallestGeneratedString;

// LeetCode 3474. Lexicographically Smallest Generated String: build the
// shortest word (length n + m - 1) where str1[i] == 'T' forces
// word[i..i+m) == str2 and str1[i] == 'F' forbids it, lexicographically
// smallest, or "" if the 'T' constraints alone are already contradictory.
//
// Greedy in two passes over a char[] seeded with 'a' (the smallest possible
// character, so anything never touched by a 'T' is already optimal):
//   1. Every 'T' overwrites its window with str2. Two overlapping 'T'
//      windows are consistent exactly when str2 shifted by their gap agrees
//      with itself - the two strategies below differ only in how that
//      agreement is checked.
//   2. Every 'F' whose window still happens to equal str2 (because every
//      position in it was either untouched-'a' matching str2, or fixed to
//      match by the 'T' pass) gets exactly one character bumped from 'a' to
//      'b' - the rightmost untouched position in the window, so the change
//      lands as late (least significant) as possible and the result stays
//      lexicographically smallest. If every position in the window was fixed
//      by some 'T', there is no room to bump and the word is impossible.
internal static class LexicographicallySmallestGeneratedStringSolution
{
    private const char DefaultChar = 'a';
    private const char AlternateChar = 'b';

    // The textbook form: no precomputation: a 'T' pair's overlap is verified
    // by comparing the already-written characters directly (an O(m) walk per
    // 'T', same cost as writing it) rather than reasoning about str2's own
    // self-overlap. O(n*m) overall - the arm the Z-function strategy below
    // has to justify itself against.
    public static string GenerateStringByDirectFill(string str1, string str2)
    {
        var word = Seed(str1.Length, str2.Length);
        var fixedByT = new bool[word.Length];

        for (var i = 0; i < str1.Length; i++)
        {
            if (str1[i] != 'T')
            {
                continue;
            }

            for (var j = 0; j < str2.Length; j++)
            {
                var k = i + j;

                if (fixedByT[k] && word[k] != str2[j])
                {
                    return string.Empty;
                }

                word[k] = str2[j];
                fixedByT[k] = true;
            }
        }

        return ApplyForbiddenWindows(word, fixedByT, str1, str2);
    }

    // Same two-pass shape, but a 'T' window's overlap with the previous one is
    // verified in O(1) via this repo's own ZFunction.Compute(str2): str2
    // shifted by gap agrees with itself exactly when selfOverlap[gap] covers
    // the whole remaining overlap - instead of re-comparing characters. Each
    // window's characters are also written at most once (only the portion
    // past the previous window's end), so the fill pass is O(n + m) rather
    // than O(n*m); only the forbidden-window pass below still costs O(n*m).
    public static string GenerateStringByZFunctionConsistency(string str1, string str2)
    {
        var word = Seed(str1.Length, str2.Length);
        var fixedByT = new bool[word.Length];
        var selfOverlap = ZFunction.Compute(str2);

        var lastT = -1;
        var filledThrough = 0;

        for (var i = 0; i < str1.Length; i++)
        {
            if (str1[i] != 'T')
            {
                continue;
            }

            if (lastT >= 0)
            {
                var gap = i - lastT;

                if (gap < str2.Length && selfOverlap[gap] < str2.Length - gap)
                {
                    return string.Empty;
                }
            }

            for (var k = Math.Max(i, filledThrough); k < i + str2.Length; k++)
            {
                word[k] = str2[k - i];
                fixedByT[k] = true;
            }

            filledThrough = Math.Max(filledThrough, i + str2.Length);
            lastT = i;
        }

        return ApplyForbiddenWindows(word, fixedByT, str1, str2);
    }

    private static char[] Seed(int n, int m)
    {
        var word = new char[n + m - 1];
        Array.Fill(word, DefaultChar);
        return word;
    }

    private static string ApplyForbiddenWindows(char[] word, bool[] fixedByT, string str1, string str2)
    {
        for (var i = 0; i < str1.Length; i++)
        {
            if (str1[i] != 'F' || !MatchesPattern(word, i, str2))
            {
                continue;
            }

            if (!TryBreakMatch(word, fixedByT, i, str2.Length))
            {
                return string.Empty;
            }
        }

        return new string(word);
    }

    private static bool MatchesPattern(char[] word, int start, string str2)
    {
        for (var j = 0; j < str2.Length; j++)
        {
            if (word[start + j] != str2[j])
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryBreakMatch(char[] word, bool[] fixedByT, int start, int length)
    {
        for (var k = start + length - 1; k >= start; k--)
        {
            if (!fixedByT[k])
            {
                word[k] = AlternateChar;
                return true;
            }
        }

        return false;
    }
}
