using DSAExperimentation.DataStructures.LazySegmentTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FancySequence;

// The lazy-propagation algebra LC 1622 needs: a pending update is the affine
// transform x -> Mult*x + Add, which is exactly what addAll (Mult=1) and multAll
// (Add=0) each are, and the composition of two affine transforms is again affine -
// the closure property that lets LazySegmentTree fold a newly-arriving update onto
// one already pending instead of pushing the old one all the way down first.
//
// ComposeUpdate is plain function composition: outer(inner(x)) = outer.Mult*inner.Mult*x
// + (outer.Mult*inner.Add + outer.Add).
//
// Identity/Combine are never actually read back by the point-only queries
// FancySequenceSolution issues (LazySegmentTree.Query's full-cover branch only returns
// a node's own value for a query spanning the node's whole range, which for a
// single-leaf query only ever happens at the leaf itself), so any associative pairing
// satisfies the algebra - plain mod-sum mirrors RangeAddSumOperation's own choice.
//
// NoUpdate = (1, 0) is the affine identity x -> x, which is also why
// FancySequenceSolution skips an addAll(0) or a multAll(1) instead of forwarding it:
// LazySegmentTree reserves NoUpdate to mean "nothing pending" and rejects it as an
// explicit update.
//
// This algebra answers one LeetCode problem and nothing else - it fixes 1e9+7 into
// every operation - which is why it lives beside the solution rather than in
// DataStructures/LazySegmentTree beside RangeAddSumOperation/RangeAssignMaxOperation.
internal readonly struct AffineOperation : IRangeUpdateOperation<long, (long Mult, long Add)>
{
    public static long Identity => 0L;

    public static (long Mult, long Add) NoUpdate => (1L, 0L);

    public static long Combine(long left, long right) => (left + right) % ModularArithmetic.Modulo;

    public static (long Mult, long Add) ComposeUpdate((long Mult, long Add) outer, (long Mult, long Add) inner)
        => (outer.Mult * inner.Mult % ModularArithmetic.Modulo,
            (outer.Mult * inner.Add + outer.Add) % ModularArithmetic.Modulo);

    public static long ApplyUpdate(long aggregate, (long Mult, long Add) update, int rangeLength)
        => (update.Mult * aggregate + update.Add) % ModularArithmetic.Modulo;
}
