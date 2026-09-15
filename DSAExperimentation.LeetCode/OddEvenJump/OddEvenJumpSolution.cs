using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using JumpIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.OddEvenJump;

// LeetCode 975. Odd Even Jump: from a starting index, odd-numbered jumps land on the
// nearest later index holding the smallest value >= the current one, even-numbered
// jumps on the nearest later index holding the largest value <= it (ties broken toward
// the smaller index). Count the starting indices from which alternating jumps can
// reach the last index.
//
// Both strategies split the problem the same way - first resolve each index's odd and
// even jump target, then run one backward odd/even reachability pass - and differ only
// in how the jump targets are found:
//
//   BruteForceScan is the textbook answer: for each index scan every later index and
//   keep the best qualifying one. O(n^2), all BCL.
//
//   MergeSortStackSweep orders the indices by (value, index) with this repo's own
//   MergeSort.Sort<int, ArrayIndexedSequence<int>> - ascending for odd jumps,
//   descending for even, always tie-broken toward the smaller index - then sweeps that
//   order once with a monotonic Stack<int> of positions still waiting for a target (the
//   DailyTemperatures precedent). An index reached later in the sort order already
//   qualifies for every pending position to its left, so each pop resolves one target
//   in O(1), for O(n log n) overall.
internal static class OddEvenJumpSolution
{
    // An index with no qualifying later index to jump to.
    private const int NoJump = -1;

    // The backward reachability fill starts one index before the last, since the last
    // index is the base case and already true in both directions.
    private const int BackwardScanStartOffset = 2;

    private readonly record struct JumpTargets(int[] OddNext, int[] EvenNext);

    private readonly record struct Reachability(bool[] Odd, bool[] Even);

    // The textbook arm, deliberately all-BCL: a quadratic forward scan per index.
    public static int OddEvenJumpsByBruteForceScan(int[] arr) =>
        CountGoodStarts(new JumpTargets(
            BruteNext(arr, JumpDirection.Odd),
            BruteNext(arr, JumpDirection.Even)));

    // For each index, scans every later index directly for the smallest qualifying
    // value (odd jumps) or the largest qualifying value (even jumps), ties broken
    // toward the nearer index - the O(n^2) baseline the sweep below replaces.
    private static int[] BruteNext(int[] arr, JumpDirection direction)
    {
        var next = InitializeNext(arr.Length);

        for (var i = 0; i < arr.Length; i++)
        {
            next[i] = FindNextIndex(arr, i, direction);
        }

        return next;
    }

    private static int FindNextIndex(int[] arr, int i, JumpDirection direction)
    {
        var best = NoJump;

        for (var j = i + 1; j < arr.Length; j++)
        {
            var qualifies = QualifiesAsJumpTarget(arr[j], arr[i], direction);

            if (!qualifies)
            {
                continue;
            }

            if (best == NoJump || BeatsCurrentTarget(arr[j], arr[best], direction))
            {
                best = j;
            }
        }

        return best;
    }

    // Whether j qualifies as a jump target from i under this direction: the nearest
    // later index holding a value at least i's (odd jumps) or at most i's (even).
    private static bool QualifiesAsJumpTarget(int candidate, int origin, JumpDirection direction)
    {
        if (direction == JumpDirection.Odd)
        {
            return candidate >= origin;
        }

        return candidate <= origin;
    }

    // Whether j displaces the best target found so far under this direction: a smaller
    // value than the best one for odd jumps, a larger one for even.
    private static bool BeatsCurrentTarget(int candidate, int best, JumpDirection direction)
    {
        if (direction == JumpDirection.Odd)
        {
            return candidate < best;
        }

        return candidate > best;
    }

    // The composed arm: MergeSort over the index order plus one monotonic Stack<int>
    // sweep per jump direction.
    public static int OddEvenJumpsByMergeSortStackSweep(int[] arr) =>
        CountGoodStarts(new JumpTargets(
            SortedNext(arr, JumpDirection.Odd),
            SortedNext(arr, JumpDirection.Even)));

    private static int[] SortedNext(int[] arr, JumpDirection direction)
    {
        var indices = BuildSortedIndices(arr, direction);
        var next = InitializeNext(arr.Length);

        FillNextViaStackSweep(indices, next);

        return next;
    }

    private static int[] BuildSortedIndices(int[] arr, JumpDirection direction)
    {
        var indices = Enumerable.Range(0, arr.Length).ToArray();
        var comparer = BuildIndexComparer(arr, direction);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices;
    }

    private static Comparer<int> BuildIndexComparer(int[] arr, JumpDirection direction) =>
        direction == JumpDirection.Odd
            ? Comparer<int>.Create((a, b) => CompareForOddJump(arr, a, b))
            : Comparer<int>.Create((a, b) => CompareForEvenJump(arr, a, b));

    // Odd jumps order ascending by value, even jumps descending, both tie-broken toward
    // the smaller index so the nearer of two equal-valued targets wins.
    private static int CompareForOddJump(int[] arr, int a, int b)
    {
        var valuesDiffer = arr[a] != arr[b];

        return valuesDiffer ? arr[a].CompareTo(arr[b]) : a.CompareTo(b);
    }

    private static int CompareForEvenJump(int[] arr, int a, int b)
    {
        var valuesDiffer = arr[a] != arr[b];

        return valuesDiffer ? arr[b].CompareTo(arr[a]) : a.CompareTo(b);
    }

    private static void FillNextViaStackSweep(int[] indices, int[] next)
    {
        var pending = new JumpIndexStack();

        foreach (var i in indices)
        {
            PopSatisfied(pending, next, i);

            pending.Push(i);
        }
    }

    private static void PopSatisfied(JumpIndexStack pending, int[] next, int currentIndex)
    {
        while (pending.TryPeek(out var left) && left < currentIndex)
        {
            pending.TryPop(out _);
            next[left] = currentIndex;
        }
    }

    // Alternating jumps make index i a good start exactly when an odd jump from i can
    // reach the last index, so the answer is the number of true entries in Odd.
    private static int CountGoodStarts(JumpTargets targets)
    {
        var reachability = InitializeReachability(targets.OddNext.Length);

        FillReachability(targets, reachability);

        return CountTrue(reachability.Odd);
    }

    private static Reachability InitializeReachability(int n)
    {
        var odd = new bool[n];
        var even = new bool[n];
        odd[n - 1] = even[n - 1] = true;

        return new Reachability(odd, even);
    }

    private static void FillReachability(JumpTargets targets, Reachability reachability)
    {
        for (var i = targets.OddNext.Length - BackwardScanStartOffset; i >= 0; i--)
        {
            if (targets.OddNext[i] != NoJump)
            {
                reachability.Odd[i] = reachability.Even[targets.OddNext[i]];
            }

            if (targets.EvenNext[i] != NoJump)
            {
                reachability.Even[i] = reachability.Odd[targets.EvenNext[i]];
            }
        }
    }

    private static int CountTrue(bool[] values)
    {
        var count = 0;

        foreach (var value in values)
        {
            if (value)
            {
                count++;
            }
        }

        return count;
    }

    private static int[] InitializeNext(int n)
    {
        var next = new int[n];
        Array.Fill(next, NoJump);

        return next;
    }
}
