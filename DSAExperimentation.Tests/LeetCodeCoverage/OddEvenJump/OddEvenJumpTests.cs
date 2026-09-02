using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using JumpIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OddEvenJump;

// LeetCode 975. Odd Even Jump: for each direction (odd jumps land on the smallest
// value >= the current one, even jumps on the largest value <= it), this repo's own
// MergeSort.Sort<int,ArrayIndexedSequence<int>> orders the indices by (value, index)
// - ascending for odd, descending for even, ties broken toward the earlier index the
// same custom-comparer shape QueueReconstructionByHeightTests/LargestTriangleAreaTests
// already use - then a single monotonic Stack<int> sweep (DailyTemperatures/
// LargestTriangleArea precedent) over that order recovers each index's jump target in
// one O(n) pass: a later-processed index i "beats" every still-pending stack index to
// its left, since sort order already guarantees i's value qualifies. A backward
// odd[]/even[] dynamic-programming pass then counts how many starting indices can
// alternate all the way to the last index.
public sealed partial class OddEvenJumpTests
{
    [Fact]
    public void OddEvenJumps_SingleIndex_TriviallyReachesItself()
    {
        int[] arr = [5];

        var result = OddEvenJumps(arr);

        Assert.Equal(1, result);
    }

    [Fact]
    public void OddEvenJumps_ClassicExample_CountsGoodStartingIndices()
    {
        int[] arr = [2, 3, 1, 1, 4];

        var result = OddEvenJumps(arr);

        Assert.Equal(3, result);
    }

    [Fact]
    public void OddEvenJumps_SecondExample_CountsGoodStartingIndices()
    {
        int[] arr = [5, 1, 3, 4, 2];

        var result = OddEvenJumps(arr);

        Assert.Equal(3, result);
    }

    private static int OddEvenJumps(int[] arr)
    {
        var jumpTargets = ComputeJumpTargets(arr);

        return CountGoodStarts(arr.Length, jumpTargets);
    }

    private static (int[] OddNext, int[] EvenNext) ComputeJumpTargets(int[] arr)
    {
        var oddNext = NextJumpIndices(arr, ascending: true);
        var evenNext = NextJumpIndices(arr, ascending: false);

        return (oddNext, evenNext);
    }

    private static int CountGoodStarts(int n, (int[] OddNext, int[] EvenNext) jumps)
    {
        var reach = InitializeReachability(n);
        var goodStarts = 1;

        for (var i = n - 2; i >= 0; i--)
        {
            if (UpdateReachability(i, jumps, reach))
            {
                goodStarts++;
            }
        }

        return goodStarts;
    }

    private static (bool[] Odd, bool[] Even) InitializeReachability(int n)
    {
        var odd = new bool[n];
        var even = new bool[n];
        odd[n - 1] = even[n - 1] = true;

        return (odd, even);
    }

    private static bool UpdateReachability(int i, (int[] OddNext, int[] EvenNext) jumps, (bool[] Odd, bool[] Even) reach)
    {
        if (jumps.OddNext[i] != -1)
        {
            reach.Odd[i] = reach.Even[jumps.OddNext[i]];
        }

        if (jumps.EvenNext[i] != -1)
        {
            reach.Even[i] = reach.Odd[jumps.EvenNext[i]];
        }

        return reach.Odd[i];
    }

    // Sorts indices by (value, index) - ascending for odd jumps, descending for even
    // jumps, always tie-broken toward the smaller index - then sweeps that order with
    // a monotonic Stack<int> of positions still waiting for their jump target: index i
    // (processed later, so its value already qualifies) resolves every pending
    // position to its left, since that's the nearest still-open position that a
    // qualifying value can land on.
    private static int[] NextJumpIndices(int[] arr, bool ascending)
    {
        var indices = SortIndicesByJumpOrder(arr, ascending);

        return ResolveJumpTargets(indices, arr.Length);
    }

    private static int[] SortIndicesByJumpOrder(int[] arr, bool ascending)
    {
        var indices = Enumerable.Range(0, arr.Length).ToArray();
        var comparer = BuildJumpOrderComparer(arr, ascending);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices;
    }

    private static IComparer<int> BuildJumpOrderComparer(int[] arr, bool ascending) =>
        ascending
            ? Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[a].CompareTo(arr[b]) : a.CompareTo(b))
            : Comparer<int>.Create((a, b) => arr[a] != arr[b] ? arr[b].CompareTo(arr[a]) : a.CompareTo(b));

    private static int[] ResolveJumpTargets(int[] indices, int n)
    {
        var next = new int[n];
        Array.Fill(next, -1);

        var pending = new JumpIndexStack();

        foreach (var i in indices)
        {
            while (pending.TryPeek(out var left) && left < i)
            {
                pending.TryPop(out _);
                next[left] = i;
            }

            pending.Push(i);
        }

        return next;
    }
}
