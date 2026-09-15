using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.InterleavingString;

// LeetCode 97. Interleaving String: can target be formed by interleaving first and
// second, preserving each string's own internal character order?
//
// The state (i, j) - how many characters of first/second have been consumed - fixes
// the next position in target (k = i + j), so this is a single memoized recursion
// over one two-dimensional state space rather than two independent walks.
internal static class InterleavingStringSolution
{
    public static bool IsInterleaveByMemoizedRecursion(string first, string second, TargetText target)
    {
        if (first.Length + second.Length != target.Text.Length)
        {
            return false;
        }

        var recurrence = new InterleavingFromOffsets(first, second, target);

        return Memoizer.Memoize<(int First, int Second), bool>((0, 0), recurrence);
    }

    // The recurrence, named: target is fully consumed once the two cursors reach its
    // end, and otherwise the next character comes from whichever string still has it.
    private sealed class InterleavingFromOffsets(string first, string second, TargetText target)
        : IRecurrence<(int First, int Second), bool>
    {
        /// <inheritdoc/>
        public bool Replay((int First, int Second) state, IRecurrence<(int First, int Second), bool> rest)
        {
            var (i, j) = state;
            var k = i + j;

            if (k == target.Text.Length)
            {
                return true;
            }

            var canTakeFromFirst = i < first.Length && first[i] == target.Text[k];
            var canTakeFromSecond = j < second.Length && second[j] == target.Text[k];

            return (canTakeFromFirst && rest.Replay((i + 1, j), rest))
                || (canTakeFromSecond && rest.Replay((i, j + 1), rest));
        }
    }

    // The string the interleaving has to spell. `first` and `second` are two sources a
    // caller may hand over in either order - swapping them asks the same question - but
    // `target` is what the interleaving is formed into, so it is not one of them. Naming
    // that role separately means the two strings being merged and the string they must
    // produce can no longer be transposed at a call site with the compiler none the
    // wiser.
    internal readonly record struct TargetText(string Text);
}
