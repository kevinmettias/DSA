using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.DataStructures.RangeFenwickTree;

// Composes two internal FenwickTree<Element,TOperation> instances directly (the classic two-Fenwick-
// tree range-update/range-query trick) - precedented by Collections/Queue/Queue.cs composing
// Collections/Deque/Deque.cs, itself Operations-composing-Operations rather than raw
// Representation. RangeAdd/PrefixQuery/Query are O(log n) - depends on FenwickArray<Element>.Get/Set
// being O(1) (see ARCHITECTURE.md §8) and on TOperation.Scale being O(1) or cheap (see
// IScaledGroupOperation<Element>'s own doc comment).
//
// Derivation, 0-indexed inclusive ranges: RangeAdd(l, r, val) posts val into B1's difference
// array at position l (and -val at r+1, if in bounds) the same way a plain difference-array
// range-update works, and posts the position-weighted term Scale(val, l) / Scale(-val, r+1) into
// B2. Then PrefixQuery(i) = Combine(Scale(B1.PrefixQuery(i), i+1), Invert(B2.PrefixQuery(i))) -
// the standard identity that turns a sum of per-position deltas into a prefix sum without
// re-walking every position individually.
//
// Range-update only, deliberately: no point-Add method. Point-update/range-query is FenwickTree's
// job - mixing that usage pattern onto this type would silently redefine what PrefixQuery means,
// the same trap SegmentTree/LazySegmentTree stay split to avoid. A point query is just
// Query(i, i) - no separate method needed.
internal sealed class RangeFenwickTree<Element, TOperation>
    where TOperation : struct, IScaledGroupOperation<Element>
{
    private const string IndexOutOfBoundsMessage = "Index was outside the bounds of the range Fenwick tree.";
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the range Fenwick tree, or left exceeded right.";

    private readonly FenwickTree<Element, TOperation> _b1;
    private readonly FenwickTree<Element, TOperation> _b2;
    private readonly int _size;

    public int Count => _size;

    public RangeFenwickTree(int size)
    {
        _size = size;
        _b1 = new FenwickTree<Element, TOperation>(size);
        _b2 = new FenwickTree<Element, TOperation>(size);
    }

    public RangeFenwickTree(IReadOnlyList<Element> initial)
        : this(initial.Count)
    {
        for (var index = 0; index < initial.Count; index++)
        {
            RangeAdd(index, index, initial[index]);
        }
    }

    public void RangeAdd(int left, int right, Element delta)
    {
        ValidateRange(left, right);

        ApplyDelta(left, delta);

        if (right + 1 < _size)
        {
            ApplyDelta(right + 1, TOperation.Invert(delta));
        }
    }

    private void ApplyDelta(int position, Element delta)
    {
        _b1.Add(position, delta);

        var scaledDelta = TOperation.Scale(delta, position);
        _b2.Add(position, scaledDelta);
    }

    public Element PrefixQuery(int index)
    {
        ValidateIndex(index);

        var scaledB1 = TOperation.Scale(_b1.PrefixQuery(index), index + 1);

        return TOperation.Combine(scaledB1, TOperation.Invert(_b2.PrefixQuery(index)));
    }

    private void ValidateIndex(int index) => RangeBounds.ValidateIndex(index, _size, IndexOutOfBoundsMessage);

    public Element Query(int left, int right)
    {
        ValidateRange(left, right);

        var rightPrefix = PrefixQuery(right);

        return left == 0 ? rightPrefix : TOperation.Combine(rightPrefix, TOperation.Invert(PrefixQuery(left - 1)));
    }

    private void ValidateRange(int left, int right) => RangeBounds.ValidateRange(left, right, _size, RangeOutOfBoundsMessage);
}
