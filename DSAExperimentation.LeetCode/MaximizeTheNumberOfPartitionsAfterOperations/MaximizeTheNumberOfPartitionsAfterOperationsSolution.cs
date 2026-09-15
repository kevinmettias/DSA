using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximizeTheNumberOfPartitionsAfterOperations;

// LeetCode 3003. Maximize the Number of Partitions After Operations: change at
// most one character of s, then repeatedly strip the longest prefix containing at
// most k distinct characters as one partition until s is empty. Maximize the
// resulting partition count.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MaximizeTheNumberOfPartitionsAfterOperationsSolution
{
    private const int AlphabetSize = 26;

    // Whether the one permitted character change is still available is a state the
    // recursion is defined over, not a flag: naming the two cases keeps every seed
    // and every step of the memo walk self-describing.
    private enum ChangeAvailability
    {
        Available,
        Spent,
    }

    // The textbook baseline: try "no change" plus every one of the (at most)
    // 25 * Length single-character recolorings, and for each candidate string run
    // the greedy O(Length) partition walk directly - the O(Length^2 * 26) arm the
    // bitmask DP below has to beat.
    public static int MaxPartitionsByBruteForceRecolor(string s, int k)
    {
        var best = CountPartitions(s, k);
        var text = (Chars: s.ToCharArray(), Original: s);

        for (var i = 0; i < text.Chars.Length; i++)
        {
            var recolored = BestOverRecolorings(text, i, k);

            best = Math.Max(best, recolored);
        }

        return best;
    }

    // The best partition count over every one-character recolouring of position
    // `index`: each candidate letter is written into the working array, counted by the
    // same greedy walk the no-change arm uses, and the original letter goes back in
    // place so the next position starts from the untouched string again.
    private static int BestOverRecolorings(
        (char[] Chars, string Original) text, int index, int distinctLimit)
    {
        var best = 0;

        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            var candidate = (char)('a' + letter);

            if (candidate == text.Original[index])
            {
                continue;
            }

            text.Chars[index] = candidate;

            var recolored = CountPartitions(new string(text.Chars), distinctLimit);

            best = Math.Max(best, recolored);
        }

        text.Chars[index] = text.Original[index];

        return best;
    }

    // Memoizer walks the string once per reachable (position, current-partition
    // mask, change-still-available) state, trying every letter substitution right
    // where the recursion stands instead of materializing a whole new string per
    // candidate the way the brute force does. Same tuple-state
    // Memoizer.Memoize<TState,TResult> composition
    // CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo and
    // NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo already prove
    // out for unrelated counting recurrences.
    public static int MaxPartitionsByBitmaskMemo(string s, int k) =>
        Memoizer.Memoize<(int Position, int Mask, ChangeAvailability Change), int>(
            (0, 0, ChangeAvailability.Available),
            new PartitionsFromWindow(s, k));

    private static int PartitionsFrom(
        (int Position, int Mask, ChangeAvailability Change) state,
        string s,
        int k,
        IRecurrence<(int Position, int Mask, ChangeAvailability Change), int> rest)
    {
        if (state.Position == s.Length)
        {
            return state.Mask == 0 ? 0 : 1;
        }

        var originalLetter = s[state.Position] - 'a';
        var best = ExtendWith(state, originalLetter, k, rest);

        if (state.Change == ChangeAvailability.Spent)
        {
            return best;
        }

        var changed = BestOverLetters(state, originalLetter, k, rest);

        return Math.Max(best, changed);
    }

    // Every other letter tried at one position: the arm the recursion only reaches
    // while the one change is still available, so each of its steps is extended from
    // the state with that change already consumed.
    private static int BestOverLetters(
        (int Position, int Mask, ChangeAvailability Change) state,
        int originalLetter,
        int distinctLimit,
        IRecurrence<(int Position, int Mask, ChangeAvailability Change), int> rest)
    {
        var spent = (Position: state.Position, Mask: state.Mask, Change: ChangeAvailability.Spent);
        var best = 0;

        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            if (letter == originalLetter)
            {
                continue;
            }

            var extended = ExtendWith(spent, letter, distinctLimit, rest);

            best = Math.Max(best, extended);
        }

        return best;
    }

    private static int CountPartitions(string s, int k)
    {
        var walk = (Partitions: 0, Mask: 0, Distinct: 0);

        foreach (var c in s)
        {
            walk = ConsumeCharacter(walk, c, k);
        }

        return walk.Mask == 0 ? walk.Partitions : CountWithTrailingRun(walk.Partitions);
    }

    // One character appended to the partition walk. A letter already in the mask leaves
    // the walk exactly as it stands; a new letter opens a partition once the current
    // one already holds k distinct letters, and only then does the mask start over;
    // otherwise it simply joins the partition in progress.
    private static (int Partitions, int Mask, int Distinct) ConsumeCharacter(
        (int Partitions, int Mask, int Distinct) walk, char character, int distinctLimit)
    {
        var bit = 1 << (character - 'a');

        if ((walk.Mask & bit) != 0)
        {
            return walk;
        }

        if (walk.Distinct == distinctLimit)
        {
            return (walk.Partitions + 1, bit, 1);
        }

        return (walk.Partitions, walk.Mask | bit, walk.Distinct + 1);
    }

    // The walk counts every partition it closed; a non-empty mask means it stopped
    // holding characters that never filled one up, and those are a partition of
    // their own - shorter only because the string ran out.
    private static int CountWithTrailingRun(int closedPartitions) => closedPartitions + 1;

    // The single step the recursion takes at one position for one candidate letter:
    // the state it is taken from, the letter appended, and the k it must stay within.
    private static int ExtendWith(
        (int Position, int Mask, ChangeAvailability Change) state,
        int letter,
        int k,
        IRecurrence<(int Position, int Mask, ChangeAvailability Change), int> rest)
    {
        var extended = state.Mask | (1 << letter);

        // Adding this letter to the partition in progress is allowed while it still
        // leaves the partition at or below k distinct characters.
        var fitsCurrentPartition = System.Numerics.BitOperations.PopCount((uint)extended) <= k;

        return fitsCurrentPartition
            ? rest.Replay((state.Position + 1, extended, state.Change), rest)
            : CountFromNewPartition(state, letter, rest);
    }

    // The other half of that choice: taking the letter anyway means the partition in
    // progress is closed here - one more to the count - and the next partition starts
    // over holding only this letter.
    private static int CountFromNewPartition(
        (int Position, int Mask, ChangeAvailability Change) state,
        int letter,
        IRecurrence<(int Position, int Mask, ChangeAvailability Change), int> rest)
        => 1 + rest.Replay((state.Position + 1, 1 << letter, state.Change), rest);

    // The recurrence, named: how many partitions the rest of the string yields from one
    // (position, partition-in-progress mask, change-still-available) state. The string and
    // its distinct-character limit are fixed for the whole walk and arrive once through the
    // primary constructor; `rest` is the memo run's own handle on this rule, so every
    // recursive step below is a method call on a named type rather than an anonymous
    // call-back value.
    private sealed class PartitionsFromWindow(string s, int k)
        : IRecurrence<(int Position, int Mask, ChangeAvailability Change), int>
    {
        /// <inheritdoc/>
        public int Replay(
            (int Position, int Mask, ChangeAvailability Change) state,
            IRecurrence<(int Position, int Mask, ChangeAvailability Change), int> rest)
            => PartitionsFrom(state, s, k, rest);
    }
}
