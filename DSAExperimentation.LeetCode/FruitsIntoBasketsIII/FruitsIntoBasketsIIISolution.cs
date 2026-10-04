using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.ElementAlgebra;
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
            if (!TryPlaceBruteForce(used, baskets, quantity))
            {
                unplaced++;
            }
        }

        return unplaced;
    }

    // Places one fruit in the leftmost still-free basket that fits it, marking that
    // basket used - and reports whether any such basket was left.
    private static bool TryPlaceBruteForce(bool[] used, int[] baskets, int quantity)
    {
        for (var j = 0; j < baskets.Length; j++)
        {
            if (!used[j] && baskets[j] >= quantity)
            {
                used[j] = true;
                return true;
            }
        }

        return false;
    }

    // This repo's own SegmentTree<int, MaxOperation<int>> over basket capacity,
    // used baskets flattened to MaxOperation<int>.Identity (int.MinValue) so
    // they can never again satisfy a capacity >= 1 requirement - the same
    // "deactivate by writing Identity" move BlockPlacementQueriesSolution
    // makes on its own gap tree. "Leftmost basket whose capacity is >=
    // quantity" is a leftmost-index-satisfying-a-monotonic-predicate search,
    // which SegmentTree does not expose as a single tree descent - Query only
    // aggregates a range - so LeftmostBasketAtLeast hands the rule
    // Query(0, mid) >= quantity to this repo's own MonotonePredicateSearch.FirstTrue:
    // it is monotone in mid once true (widening a range can only raise its max).
    // That costs O(log^2 n) per fruit instead of O(log n) - each probe is itself a
    // Query - the price of composing the tree through its public Query/Update
    // surface rather than a bespoke descent, but still a clean asymptotic win over
    // the O(n) rescan above.
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

    // The first basket whose prefix max reaches capacity is the leftmost free basket
    // that fits it. FirstTrue's high + 1 - one past the last basket - means even the
    // whole row's max falls short, so no free basket fits at all.
    private static int? LeftmostBasketAtLeast(SegmentTree<int, MaxOperation<int>> baskets, int capacity)
    {
        var lastBasket = baskets.Count - 1;
        var basket = MonotonePredicateSearch.FirstTrue(0, lastBasket, new ReachesCapacity(baskets, capacity));

        return basket > lastBasket ? null : basket;
    }

    // Holds(last) is "some basket in [0, last] still holds at least capacity" -
    // false up to the answer and true from there on, since widening a range can only
    // raise its max. A rule for this problem alone: the fit test is LC 3479's own
    // content.
    private readonly struct ReachesCapacity(SegmentTree<int, MaxOperation<int>> baskets, int capacity)
        : IMonotonePredicate<int>
    {
        public bool Holds(int last) => baskets.Query(0, last) >= capacity;
    }
}
