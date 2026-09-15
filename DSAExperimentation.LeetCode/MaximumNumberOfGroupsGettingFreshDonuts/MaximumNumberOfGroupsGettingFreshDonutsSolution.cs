using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumNumberOfGroupsGettingFreshDonuts;

// LeetCode 1815. Maximum Number of Groups Getting Fresh Donuts: choose the serving
// order that maximizes how many groups arrive when the running total of donuts
// served is an exact multiple of batchSize (a "happy" group; the first group is
// always happy, since the running total starts at zero).
//
// Only a group's size modulo batchSize matters. A group whose size is already a
// multiple of batchSize leaves the running residue untouched, so serving all of
// them first makes every one of them happy for free; the rest matter only through
// how many of each non-zero remainder there are.
//
// The two strategies answer that from opposite ends. The baseline scores every one
// of the n! orderings; the composed strategy recognizes that all orderings sharing a
// (running residue, remaining remainder counts) state have the same future, and
// drives that recurrence through this repo's own Memoizer - the same top-down
// memoized search CountAllPossibleRoutes uses, over a different state shape.
internal static class MaximumNumberOfGroupsGettingFreshDonutsSolution
{
    // State is encoded as "residue|c1,c2,..." because a bare int[] has no structural
    // equality of its own to hand Memoizer's dictionary cache.
    private const string ResidueSeparator = "|";
    private const char CountSeparator = ',';

    // The textbook answer: try every permutation of the groups and score each one.
    // O(n! * n), BCL only - the arm the memoized strategy below has to justify
    // itself against.
    public static int MaxHappyGroupsByAllPermutations(int batchSize, int[] groups)
    {
        // Permute swaps in place, so it walks a private copy rather than the
        // caller's array.
        var order = (int[])groups.Clone();
        var best = 0;

        Permute(order, batchSize, 0, ref best);

        return best;
    }

    // The same maximum, reached by collapsing every ordering that leaves the search
    // in the same (residue, remaining remainder counts) state onto one memoized
    // subproblem.
    public static int MaxHappyGroupsByMemoizedRecurrence(int batchSize, int[] groups)
    {
        var counts = new int[batchSize];

        foreach (var size in groups)
        {
            counts[size % batchSize]++;
        }

        // Groups whose size is a whole number of batches never move the residue, so
        // serving them first makes all of them happy and costs the rest nothing.
        var happyFromWholeBatches = counts[0];
        var remainderCounts = counts[1..];

        if (Array.TrueForAll(remainderCounts, count => count == 0))
        {
            return happyFromWholeBatches;
        }

        var initialState = EncodeState(0, remainderCounts);

        return happyFromWholeBatches +
            Memoizer.Memoize<string, int>(initialState, new BestExtraFromGroups(batchSize));
    }

    private static int BestExtraFrom(string state, IRecurrence<string, int> rest, int batchSize)
    {
        var (residue, remaining) = DecodeState(state);
        var happyHere = residue == 0 ? 1 : 0;

        return MaxExtraFromRemainders((residue, happyHere, batchSize), remaining, rest);
    }

    private static (int Residue, int[] Counts) DecodeState(string state)
    {
        var parts = state.Split(ResidueSeparator);

        return (int.Parse(parts[0]), Array.ConvertAll(parts[1].Split(CountSeparator), int.Parse));
    }

    // Every ordering of the groups still to serve, scored from the residue this
    // one leaves behind; `remaining[index]` counts the groups whose size leaves
    // remainder index + 1.
    private static int MaxExtraFromRemainders(
        (int Residue, int HappyHere, int BatchSize) context,
        int[] remaining,
        IRecurrence<string, int> rest)
    {
        var result = 0;

        for (var index = 0; index < remaining.Length; index++)
        {
            if (remaining[index] == 0)
            {
                continue;
            }

            var served = index + 1;
            var next = (int[])remaining.Clone();
            next[index]--;

            var nextState = EncodeState((context.Residue + served) % context.BatchSize, next);
            result = Math.Max(result, context.HappyHere + rest.Replay(nextState, rest));
        }

        return result;
    }

    private static string EncodeState(int residue, int[] counts) =>
        residue + ResidueSeparator + string.Join(CountSeparator, counts);

    private static void Permute(int[] order, int batchSize, int start, ref int best)
    {
        if (start == order.Length)
        {
            var happy = CountHappy(order, batchSize);
            best = Math.Max(best, happy);
            return;
        }

        for (var i = start; i < order.Length; i++)
        {
            (order[start], order[i]) = (order[i], order[start]);
            Permute(order, batchSize, start + 1, ref best);
            (order[start], order[i]) = (order[i], order[start]);
        }
    }

    private static int CountHappy(int[] order, int batchSize)
    {
        var running = 0;
        var happy = 0;

        foreach (var size in order)
        {
            if (running % batchSize == 0)
            {
                happy++;
            }

            running += size;
        }

        return happy;
    }

    // The serving rule, named: from one (residue, remaining remainder counts) state, the
    // most further happy groups any order of what is left can produce. The batch size is
    // fixed for the whole search and arrives once through the primary constructor; `rest`
    // is the memo run's own handle on this rule, so each candidate order recurses through a
    // call on a named type rather than an anonymous call-back value.
    private sealed class BestExtraFromGroups(int batchSize) : IRecurrence<string, int>
    {
        /// <inheritdoc/>
        public int Replay(string state, IRecurrence<string, int> rest)
            => BestExtraFrom(state, rest, batchSize);
    }
}
