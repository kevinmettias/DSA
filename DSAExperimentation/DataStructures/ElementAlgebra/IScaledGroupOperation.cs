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
/// <summary>
/// A group operation that can also combine many copies of one element at once, as range-update structures require.
/// </summary>
public interface IScaledGroupOperation<Element> : IGroupOperation<Element>
{
    /// <summary>
    /// Combines a number of copies of an element, which must equal repeated Combine and cost O(1) or close to it.
    /// </summary>
    /// <param name="value">The element to repeat.</param>
    /// <param name="count">How many copies to combine; 0 or more.</param>
    /// <returns>The copies combined, the identity when <paramref name="count"/> is 0.</returns>
    static abstract Element Scale(Element value, long count);
}
