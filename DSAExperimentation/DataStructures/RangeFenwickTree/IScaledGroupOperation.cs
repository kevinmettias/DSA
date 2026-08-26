using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.DataStructures.RangeFenwickTree;

// Extends FenwickTree.IGroupOperation<Element> - same-domain-family refinement (RangeFenwickTree
// composes FenwickTree directly, see RangeFenwickTree.cs), the same shape Graph's own
// ITreeTopology : IDagTopology refinement chain uses within one domain - not the cross-domain
// reuse ARCHITECTURE.md §5 step 5 forbids.
//
// Scale(value, count) must equal Combine-ing `value` with itself `count` times (repeated-addition
// style scalar multiplication over this group) - needed because the two-Fenwick-tree
// range-update/range-query trick's prefix formula multiplies a running combined value by an
// integer position count. This is a Complexity-law-shaped obligation on the contract, flagged as
// new territory rather than repo-precedented: "must be O(1) or cheap - e.g. real multiplication
// for numeric sum - not synthesized via O(log count) repeated doubling, which would silently
// degrade RangeFenwickTree's advertised O(log n) per operation to O(log n log count)." Every
// existing Complexity-law example in this repo (HeapArray.Get, IRandomAccessSequence.Get) is a
// Representation-layer cost; this is the first one placed on a plain-runtime-law-shaped witness
// instead, because there's no Representation layer here to put it on.
internal interface IScaledGroupOperation<Element> : IGroupOperation<Element>
{
    static abstract Element Scale(Element value, long count);
}
