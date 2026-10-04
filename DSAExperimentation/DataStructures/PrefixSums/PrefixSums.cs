using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.DataStructures.PrefixSums;

// Running totals of a fixed sequence under a group operation. TotalBefore(end) combines values[0, end),
// so TotalBefore(0) is Identity, and Query(left, right) answers the inclusive range as
// Combine(TotalBefore(right + 1), Invert(TotalBefore(left))) - FenwickTree.Query's convention, at O(1)
// because nothing changes after construction. FenwickTree is the choice once values do; this is what
// the dozens of solutions writing prefix[i + 1] = prefix[i] + nums[i] were building by hand.
//
// There is deliberately no PrefixQuery: FenwickTree's includes its index, and an exclusive one under the
// same name is exactly the off-by-one this type removes. "Before" is in the name so the boundary is read
// off the call site, and Query is inclusive at both ends like every range structure beside it.
//
// TOperation must be a group, not a monoid, for the same reason as FenwickTree's (see IGroupOperation):
// a range is a difference of two totals. SumOperation and XorOperation are the two that ship. The
// element type is the caller's choice of overflow range, not this type's: an int[] whose sums need long
// is widened by the caller before construction, as choosing SumOperation<long> already is.
//
// Totals hands the Count + 1 running totals to BinarySearch as an ArraySequence view, composing
// Sequence's public type rather than implementing its contract (§9.4's DynamicArraySequence shape).
// They are sorted only when every term moves the total one way - non-negative addends under
// SumOperation, never in general under XorOperation - a precondition that is the caller's, as
// BinarySearch's sortedness always is.
internal sealed class PrefixSums<Element, TOperation>
    where TOperation : struct, IGroupOperation<Element>
{
    private const string EndOutOfBoundsMessage = "end was outside [0, Count] for these prefix sums.";
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of these prefix sums, or left exceeded right.";

    // _totals[end] is the combined value of the first `end` values; one longer than the input.
    private readonly Element[] _totals;

    public int Count => _totals.Length - 1;

    public ArraySequence<Element> Totals => new(_totals);

    public PrefixSums(ReadOnlySpan<Element> values)
    {
        _totals = new Element[values.Length + 1];
        _totals[0] = TOperation.Identity;

        for (var index = 0; index < values.Length; index++)
        {
            _totals[index + 1] = TOperation.Combine(_totals[index], values[index]);
        }
    }

    public Element TotalBefore(int end)
    {
        RangeBounds.ValidateIndex(end, _totals.Length, EndOutOfBoundsMessage);

        return _totals[end];
    }

    public Element Query(int left, int right)
    {
        RangeBounds.ValidateRange(left, right, Count, RangeOutOfBoundsMessage);

        return TOperation.Combine(_totals[right + 1], TOperation.Invert(_totals[left]));
    }
}
