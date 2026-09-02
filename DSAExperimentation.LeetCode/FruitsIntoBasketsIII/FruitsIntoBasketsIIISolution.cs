using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.FruitsIntoBasketsIII;

// LeetCode 3479. Fruits Into Baskets III: the identical placement rule as LC
// 3477 (Fruits Into Baskets II), but n up to 1e5 rules out II's O(n^2) rescan.
internal static class FruitsIntoBasketsIIISolution
{
    // The textbook O(n^2) form II already uses in full: rescan every basket,
    // left to right, for each fruit. The arm the segment-tree strategy below
    // has to beat.
    public static int CountUnplacedByBruteForce(int[] fruits, int[] baskets)
    {
        var used = new bool[baskets.Length];
        var unplaced = 0;

        foreach (var quantity in fruits)
        {
            var placed = false;

            for (var j = 0; j < baskets.Length; j++)
            {
                if (!used[j] && baskets[j] >= quantity)
                {
                    used[j] = true;
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                unplaced++;
            }
        }

        return unplaced;
    }

    // This repo's own SegmentTree<int, MaxOperation<int>> over basket capacity,
    // used baskets flattened to MaxOperation<int>.Identity (int.MinValue) so
    // they can never again satisfy a capacity >= 1 requirement - the same
    // "deactivate by writing Identity" move BlockPlacementQueriesSolution
    // makes on its own gap tree. "Leftmost basket whose capacity is >=
    // quantity" is a leftmost-index-satisfying-a-monotonic-predicate search,
    // which SegmentTree does not expose as a single tree descent - Query only
    // aggregates a range - so LeftmostBasketAtLeast binary-searches the split
    // point directly: Query(low, mid) >= quantity is monotonic in mid once
    // true (widening a range can only raise its max), the same shape
    // BinarySearch.LowerBound exploits over a sorted sequence. That costs
    // O(log^2 n) per fruit instead of O(log n) - each probe is itself a Query -
    // the price of composing the tree through its public Query/Update surface
    // rather than a bespoke descent, but still a clean asymptotic win over the
    // O(n) rescan above.
    public static int CountUnplacedBySegmentTreeSearch(int[] fruits, int[] baskets)
    {
        var tree = new SegmentTree<int, MaxOperation<int>>(baskets);
        var unplaced = 0;

        foreach (var quantity in fruits)
        {
            var basket = LeftmostBasketAtLeast(tree, quantity);

            if (basket is int index)
            {
                tree.Update(index, MaxOperation<int>.Identity);
            }
            else
            {
                unplaced++;
            }
        }

        return unplaced;
    }

    private static int? LeftmostBasketAtLeast(SegmentTree<int, MaxOperation<int>> baskets, int capacity)
    {
        if (baskets.Query(0, baskets.Count - 1) < capacity)
        {
            return null;
        }

        var low = 0;
        var high = baskets.Count - 1;

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (baskets.Query(low, mid) >= capacity)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }
}
