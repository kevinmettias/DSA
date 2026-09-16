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
    public static string GenerateStringByDirectFill(ConstraintPattern str1, TemplateWord str2)
    {
        var word = Seed(str1.Text.Length, str2.Text.Length);
        var fixedByT = new bool[word.Length];

        for (var i = 0; i < str1.Text.Length; i++)
        {
            if (str1.Text[i] != 'T')
            {
                continue;
            }

            if (!TryWriteWindow(word, fixedByT, i, str2.Text))
            {
                return string.Empty;
            }
        }

        return ApplyForbiddenWindows(word, fixedByT, str1, str2);
    }

    // Write str2 across the window starting at `start`, reporting false as soon as a
    // position an earlier 'T' already fixed disagrees with str2 there.
    private static bool TryWriteWindow(char[] word, bool[] fixedByT, int start, string str2)
    {
        for (var j = 0; j < str2.Length; j++)
        {
            var k = start + j;

            if (fixedByT[k] && word[k] != str2[j])
            {
                return false;
            }

            word[k] = str2[j];
            fixedByT[k] = true;
        }

        return true;
    }

    // Same two-pass shape, but a 'T' window's overlap with the previous one is
    // verified in O(1) via this repo's own ZFunction.Compute(str2): str2
    // shifted by gap agrees with itself exactly when selfOverlap[gap] covers
    // the whole remaining overlap - instead of re-comparing characters. Each
    // window's characters are also written at most once (only the portion
    // past the previous window's end), so the fill pass is O(n + m) rather
    // than O(n*m); only the forbidden-window pass below still costs O(n*m).
    public static string GenerateStringByZFunctionConsistency(ConstraintPattern str1, TemplateWord str2)
    {
        var word = Seed(str1.Text.Length, str2.Text.Length);
        var fixedByT = new bool[word.Length];
        var selfOverlap = ZFunction.Compute(str2.Text);

        var lastT = -1;
        var filledThrough = 0;

        foreach (var i in TPositions(str1.Text))
        {
            if (HasConflictWithPreviousWindow(i, lastT, selfOverlap, str2.Text))
            {
                return string.Empty;
            }

            FillWindowFrom(word, fixedByT, (i, filledThrough), str2.Text);
            (filledThrough, lastT) = (Math.Max(filledThrough, i + str2.Text.Length), i);
        }

        return ApplyForbiddenWindows(word, fixedByT, str1, str2);
    }

    // The indices where str1 demands a window - the only positions the fill pass acts
    // on.
    private static IEnumerable<int> TPositions(string str1)
    {
        for (var i = 0; i < str1.Length; i++)
        {
            if (str1[i] == 'T')
            {
                yield return i;
            }
        }
    }

    // Whether this 'T' window contradicts the previous one: str2 shifted by their gap
    // must agree with itself across the whole overlap, which selfOverlap[gap] reports
    // in one lookup. The first window overlaps nothing.
    private static bool HasConflictWithPreviousWindow(
        int windowStart, int lastT, int[] selfOverlap, string str2)
    {
        if (lastT < 0)
        {
            return false;
        }

        var gap = windowStart - lastT;

        return gap < str2.Length && selfOverlap[gap] < str2.Length - gap;
    }

    // Write only the part of str2's window that no earlier window already covered.
    private static void FillWindowFrom(
        char[] word, bool[] fixedByT, (int Start, int FilledThrough) window, string str2)
    {
        for (var k = Math.Max(window.Start, window.FilledThrough); k < window.Start + str2.Length; k++)
        {
            word[k] = str2[k - window.Start];
            fixedByT[k] = true;
        }
    }

    private static char[] Seed(int patternLength, int templateLength)
    {
        var word = new char[patternLength + templateLength - 1];
        Array.Fill(word, DefaultChar);
        return word;
    }

    private static string ApplyForbiddenWindows(
        char[] word, bool[] fixedByT, ConstraintPattern str1, TemplateWord str2)
    {
        for (var i = 0; i < str1.Text.Length; i++)
        {
            if (str1.Text[i] != 'F' || !IsPatternMatch(word, i, str2.Text))
            {
                continue;
            }

            if (!TryBreakMatch(word, fixedByT, i, str2.Text.Length))
            {
                return string.Empty;
            }
        }

        return new string(word);
    }

    private static bool IsPatternMatch(char[] word, int start, string str2)
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

    // LC 3474's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `str1` is the 'T'/'F' pattern that demands or forbids a
    // window; `str2` is the word stamped into every 'T' window. They differ in both
    // length and meaning, so a swap asks a different question entirely.
    internal readonly record struct ConstraintPattern(string Text);

    internal readonly record struct TemplateWord(string Text);
}
