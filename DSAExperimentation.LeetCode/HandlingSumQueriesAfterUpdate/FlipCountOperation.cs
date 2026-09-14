using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.LeetCode.HandlingSumQueriesAfterUpdate;

// The lazy-propagation algebra LC 2569 needs: the aggregate a node carries is "how
// many ones this range of nums1 holds", and the only update is "flip every bit in
// this range", which is a pure toggle - so TUpdate is a plain bool meaning "a flip is
// pending here".
//
// ApplyUpdate is the whole trick: flipping k bits of which `aggregate` are ones
// leaves exactly rangeLength - aggregate ones, so a node's count can be corrected
// without ever descending to its leaves. ComposeUpdate is XOR, because two pending
// flips on the same node cancel - the closure property that lets LazySegmentTree fold
// an arriving flip onto one already queued instead of pushing the old one down first.
// NoUpdate = false is therefore also the "nothing pending" sentinel LazySegmentTree
// reserves, which is why callers only ever pass `true` as an explicit update.
//
// Combine is addition and Identity is 0: counts of ones over disjoint ranges add.
//
// This witness answers one LeetCode problem and nothing else - "count ones under
// range flips" is LC 2569's own semantics, not a general shape - so it lives beside
// the solution rather than in DataStructures/LazySegmentTree beside
// RangeAddSumOperation/RangeAssignMaxOperation, the same placement
// FancySequence's AffineOperation and LongestBalancedSubarrayII's
// BalanceRangeAddOperation already take.
internal readonly struct FlipCountOperation : IRangeUpdateOperation<int, bool>
{
    public static int Identity => 0;

    public static bool NoUpdate => false;

    public static int Combine(int left, int right) => left + right;

    public static bool ComposeUpdate(bool outer, bool inner) => outer ^ inner;

    public static int ApplyUpdate(int aggregate, bool update, int rangeLength) => update ? rangeLength - aggregate : aggregate;
}
