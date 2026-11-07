using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.FenwickTree;

// The one Fenwick tree configured for range updates (ARCHITECTURE.md §13.13): FenwickTree over the
// direct product of TOperation with itself, so every node holds a pair and one walk of either tree
// reaches both values, where the classic construction keeps two trees and walks each in turn. What
// it adds over the point tree is that type argument and the encoding below, which needs Scale, an
// integer module rather than an abelian group. The point tree has no second component to allocate,
// store or walk.
//
// The encoding, 0-indexed inclusive ranges: RangeAdd(l, r, d) adds the pair (d, Scale(d, l)) at l,
// and its inverse at r + 1 when that is in bounds - a difference array in the first component, each
// difference weighted by its position in the second. A prefix of those pairs through i, (D, W),
// gives the elements' prefix as Combine(Scale(D, i + 1), Invert(W)): a difference d added at p <= i
// reaches the i + 1 - p elements from p through i, which is Scale(d, i + 1) less Scale(d, p).
//
// RangeAdd/PrefixQuery/Query are O(log n), assuming FenwickArray's Get/Set are O(1) (§8) and
// TOperation.Scale is O(1) or cheap (IIntegerModule's own law). This type checks a range, and the
// tree checks each position it is handed, so PrefixQuery leaves its index to the tree.
//
// Range updates only, deliberately: no point Add. A point update is RangeAdd(i, i, delta). A point
// query is just Query(i, i) - no separate method needed.
/// <summary>A fixed-length sequence with range-add updates and range queries, each in O(log n).</summary>
public sealed class RangeFenwickTree<Element, TOperation>
    where TOperation : struct, IIntegerModule<Element>
{
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the range Fenwick tree, or left exceeded right.";

    // Each node's pair: the combination of the differences it covers, and of those differences
    // weighted by their positions.
    private readonly FenwickTree<
        (Element Difference, Element Weighted),
        DirectProduct<Element, Element, TOperation, TOperation>> _pairs;

    private readonly int _size;

    /// <summary>The number of elements, fixed at construction.</summary>
    public int Count => _size;

    public RangeFenwickTree(int size)
    {
        _size = size;
        _pairs = new(size);
    }

    public RangeFenwickTree(IReadOnlyList<Element> initial)
        : this(initial.Count)
    {
        for (var index = 0; index < initial.Count; index++)
        {
            RangeAdd(index, index, initial[index]);
        }
    }

    /// <summary>Combines a delta into every element of the inclusive range [left, right].</summary>
    /// <param name="left">The 0-based index of the range's first element.</param>
    /// <param name="right">The 0-based index of the range's last element.</param>
    /// <param name="delta">The value to combine into each element of the range.</param>
    /// <exception cref="ArgumentOutOfRangeException">The range is reversed or reaches outside [0, Count).</exception>
    public void RangeAdd(int left, int right, Element delta)
    {
        ValidateRange(left, right);

        AddDifference(left, delta);

        if (right + 1 < _size)
        {
            AddDifference(right + 1, TOperation.Invert(delta));
        }
    }

    private void AddDifference(int position, Element delta)
    {
        var weighted = TOperation.Scale(delta, position);
        _pairs.Add(position, (delta, weighted));
    }

    /// <summary>Combines every element from index 0 through <paramref name="index"/>.</summary>
    /// <param name="index">The 0-based index of the last element to include.</param>
    /// <returns>The combined value of elements 0 through <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, Count).</exception>
    public Element PrefixQuery(int index)
    {
        var (difference, weighted) = _pairs.PrefixQuery(index);
        var scaled = TOperation.Scale(difference, index + 1);

        return TOperation.Combine(scaled, TOperation.Invert(weighted));
    }

    /// <summary>Combines the elements of the inclusive range [left, right].</summary>
    /// <param name="left">The 0-based index of the range's first element.</param>
    /// <param name="right">The 0-based index of the range's last element.</param>
    /// <returns>The combined value of the range, so <c>Query(i, i)</c> reads element i alone.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The range is reversed or reaches outside [0, Count).</exception>
    public Element Query(int left, int right)
    {
        ValidateRange(left, right);

        var rightPrefix = PrefixQuery(right);

        return left == 0 ? rightPrefix : TOperation.Combine(rightPrefix, TOperation.Invert(PrefixQuery(left - 1)));
    }

    private void ValidateRange(int left, int right) => RangeBounds.ValidateRange(left, right, _size, RangeOutOfBoundsMessage);
}
