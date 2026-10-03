namespace DSAExperimentation.DataStructures.ElementAlgebra;

// A group with a Scale that must equal Combine-ing `value` with itself `count` times - repeated-
// addition scalar multiplication over the group. RangeFenwickTree needs it because the two-Fenwick-
// tree range-update/range-query trick multiplies a running combined value by an integer position
// count.
//
// Complexity law, obligation stated here and consequence on RangeFenwickTree: Scale must be O(1) or
// cheap - real multiplication for a numeric sum, not O(log count) repeated doubling - or the tree's
// advertised O(log n) per operation silently degrades to O(log n log count). Every other Complexity
// law in this repo sits on a Representation layer (HeapArray.Get, IRandomAccessSequence.Get); this
// one sits on a witness because there is no Representation layer here to put it on.
internal interface IScaledGroupOperation<Element> : IGroupOperation<Element>
{
    static abstract Element Scale(Element value, long count);
}
