using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.FenwickTree;

// Add/PrefixQuery/Query are O(log n) - depends on FenwickArray<Element>.Get/Set being O(1). See
// ARCHITECTURE.md §8.
//
// TOperation must be an abelian group, not merely associative - see IAbelianGroup<Element>'s own doc
// comment for why RangeQuery's use of Invert forces this. Unenforced precondition law, same shape
// as BinarySearch's sortedness: an operation with no real inverse still compiles and runs, just
// returns a silently wrong Query result.
//
// Add, not Set, deliberately: Fenwick's native operation is "combine this delta into every
// ancestor," which only makes sense as an increment, not an overwrite (a Set would need first
// reading the old value out via a PrefixQuery-shaped lookup this type intentionally keeps out of
// scope). Point-update/range-query only - see RangeFenwickTree for the range-update/range-query
// variant; mixing that scheme's usage pattern onto this type would silently redefine what
// PrefixQuery even means, the same trap SegmentTree/LazySegmentTree stay split to avoid.
/// <summary>
/// A Fenwick tree over an abelian group operation that adds to one element or combines a range in O(log n).
/// </summary>
public sealed class FenwickTree<Element, TOperation>
    where TOperation : struct, IAbelianGroup<Element>
{
    private const string IndexOutOfBoundsMessage = "Index was outside the bounds of the Fenwick tree.";
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the Fenwick tree, or left exceeded right.";

    private readonly FenwickArray<Element> _nodes;
    private readonly int _size;

    /// <summary>The number of elements, fixed when the tree is built.</summary>
    public int Count => _size;

    public FenwickTree(int size)
    {
        _size = size;
        _nodes = new FenwickArray<Element>(size);

        for (var oneBasedIndex = 1; oneBasedIndex <= size; oneBasedIndex++)
        {
            _nodes.Set(oneBasedIndex, TOperation.Identity);
        }
    }

    public FenwickTree(IReadOnlyList<Element> initial)
        : this(initial.Count)
    {
        for (var index = 0; index < initial.Count; index++)
        {
            Add(index, initial[index]);
        }
    }

    /// <summary>Combines a delta into the element at an index.</summary>
    /// <param name="index">The 0-based index of the element to update.</param>
    /// <param name="delta">The value to combine into that element with the operation's Combine.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is negative or not less than <see cref="Count"/>.
    /// </exception>
    public void Add(int index, Element delta)
    {
        ValidateIndex(index);

        for (var oneBasedIndex = index + 1; oneBasedIndex <= _size; oneBasedIndex += LowestSetBit(oneBasedIndex))
        {
            var combined = TOperation.Combine(_nodes.Get(oneBasedIndex), delta);
            _nodes.Set(oneBasedIndex, combined);
        }
    }

    /// <summary>Combines every element from index 0 through a given index.</summary>
    /// <param name="index">The 0-based index of the last element to include.</param>
    /// <returns>The combined value of the elements from 0 through <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is negative or not less than <see cref="Count"/>.
    /// </exception>
    public Element PrefixQuery(int index)
    {
        ValidateIndex(index);

        var result = TOperation.Identity;

        for (var oneBasedIndex = index + 1; oneBasedIndex > 0; oneBasedIndex -= LowestSetBit(oneBasedIndex))
        {
            result = TOperation.Combine(result, _nodes.Get(oneBasedIndex));
        }

        return result;
    }

    /// <summary>Combines every element from one index through another.</summary>
    /// <param name="left">The 0-based index of the first element to include.</param>
    /// <param name="right">The 0-based index of the last element to include.</param>
    /// <returns>
    /// The combined value of the elements from <paramref name="left"/> through <paramref name="right"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="left"/> is negative, <paramref name="right"/> is not less than
    /// <see cref="Count"/>, or <paramref name="left"/> exceeds <paramref name="right"/>.
    /// </exception>
    public Element Query(int left, int right)
    {
        ValidateRange(left, right);

        var rightPrefix = PrefixQuery(right);

        return left == 0 ? rightPrefix : TOperation.Combine(rightPrefix, TOperation.Invert(PrefixQuery(left - 1)));
    }

    private void ValidateRange(int left, int right) => RangeBounds.ValidateRange(left, right, _size, RangeOutOfBoundsMessage);

    private static int LowestSetBit(int value) => value & -value;

    private void ValidateIndex(int index) => RangeBounds.ValidateIndex(index, _size, IndexOutOfBoundsMessage);
}
