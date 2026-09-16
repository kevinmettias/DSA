using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RotateString;

// LeetCode 796. Rotate String: can goal be obtained from source by some number of
// left-shifts?
//
// Both strategies rest on the same observation - every rotation of source is a
// contiguous window of source + source, so goal is a rotation iff the two are the
// same length and goal occurs somewhere inside source + source. They differ only in
// how that occurrence is looked for: a restart-on-mismatch scan, or this repo's KMP
// failure function.
internal static class RotateStringSolution
{
    // The textbook answer: double source, then scan every start position, comparing
    // character by character and restarting from goal's first character on any
    // mismatch. O(n*m). Deliberately plain BCL indexing - it is the arm the KMP
    // strategy below has to justify itself against.
    public static bool CanRotateByNaiveScan(RotationSource source, RotationGoal goal)
    {
        if (source.Text.Length != goal.Text.Length)
        {
            return false;
        }

        return HasOccurrenceByRestartingScan(new RotationSource(source.Text + source.Text), goal);
    }

    private static bool HasOccurrenceByRestartingScan(RotationSource text, RotationGoal pattern)
    {
        for (var start = 0; start + pattern.Text.Length <= text.Text.Length; start++)
        {
            if (HasMatchAt(text, pattern, start))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasMatchAt(RotationSource text, RotationGoal pattern, int start)
    {
        for (var offset = 0; offset < pattern.Text.Length; offset++)
        {
            if (text.Text[start + offset] != pattern.Text[offset])
            {
                return false;
            }
        }

        return true;
    }

    // The same containment question handed to PrefixFunctionSearch, which never
    // re-scans a character of source + source: on a mismatch it falls back through
    // goal's failure function instead of restarting, so the whole search is O(n + m).
    public static bool CanRotateByPrefixFunction(RotationSource source, RotationGoal goal)
    {
        if (source.Text.Length != goal.Text.Length)
        {
            return false;
        }

        return PrefixFunctionSearch.FindAll(source.Text + source.Text, goal.Text).Count > 0;
    }

    // The two ends of LC 796's question, named for the roles they play here rather than
    // left as two adjacent `string` positions a caller could hand over the wrong way
    // round with the compiler none the wiser. `source` is the string whose rotations
    // the scan walks - doubled, so every rotation is one of its windows - and `goal` is
    // the one those windows are compared against.
    internal readonly record struct RotationSource(string Text);

    internal readonly record struct RotationGoal(string Text);
}
