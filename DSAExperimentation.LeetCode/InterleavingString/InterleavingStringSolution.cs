using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.InterleavingString;

// LeetCode 97. Interleaving String: can target be formed by interleaving first and
// second, preserving each string's own internal character order?
//
// The state (i, j) - how many characters of first/second have been consumed - fixes
// the next position in target (k = i + j), so this is a single recursion over one
// two-dimensional state space rather than two independent walks: the memoized arm
// caches each (i, j), the roll-forward arm keeps one row.
internal static class InterleavingStringSolution
{
    // The textbook arm the memoized recurrence is measured against: the same
    // take-from-first/take-from-second rule rolled forward as one boolean row over
    // consumed-second lengths, overwritten in place for each consumed-first length.
    // It skips the Memoizer dictionary and the recursion at the same
    // O(first * second) time and O(second) space.
    public static bool IsInterleaveByIterativeTable(string first, string second, TargetText target)
    {
        if (first.Length + second.Length != target.Text.Length)
        {
            return false;
        }

        var reachable = new bool[second.Length + 1];
        reachable[0] = true;

        for (var consumedFirst = 0; consumedFirst <= first.Length; consumedFirst++)
        {
            for (var consumedSecond = 0; consumedSecond <= second.Length; consumedSecond++)
            {
                if (consumedFirst == 0 && consumedSecond == 0)
                {
                    continue;
                }

                var next = consumedFirst + consumedSecond - 1;

                var fromFirst = consumedFirst > 0
                    && first[consumedFirst - 1] == target.Text[next]
                    && reachable[consumedSecond];

                var fromSecond = consumedSecond > 0
                    && second[consumedSecond - 1] == target.Text[next]
                    && reachable[consumedSecond - 1];

                reachable[consumedSecond] = fromFirst || fromSecond;
            }
        }

        return reachable[second.Length];
    }

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
