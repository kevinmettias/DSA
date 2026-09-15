namespace DSAExperimentation.DataStructures.SegmentTree;

// Update/Query are O(log n) - a claim that assumes SegmentTreeArray<Element>.Get/Set are O(1). See
// ARCHITECTURE.md §8 and SegmentTreeArray.cs.
//
// TOperation.Combine must be associative (ICombineOperation<Element>'s own doc comment states this) -
// an unenforced precondition law, the same shape as BinarySearch's sortedness: an operation that
// isn't actually associative still builds and queries without error, just silently returns a
// wrong combined value whenever the tree's own grouping (forced by its shape) differs from
// whatever grouping the caller implicitly expected.
//
// Point-update only, deliberately: mixing a direct Update with a range-update mode on one
// instance would be a silent-wrong-answer trap (see LazySegmentTree, a separate structure for
// range-update/range-query rather than a second method here).
internal sealed class SegmentTree<Element, TOperation>
    where TOperation : struct, ICombineOperation<Element>
{
    private const string IndexOutOfBoundsMessage = "Index was outside the bounds of the segment tree.";
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the segment tree, or left exceeded right.";

    private readonly SegmentTreeArray<Element> _nodes;
    private readonly int _leafCount;

    public int Count => _leafCount;

    public SegmentTree(IReadOnlyList<Element> initial)
    {
        _leafCount = initial.Count;
        _nodes = new SegmentTreeArray<Element>(_leafCount);

        if (_leafCount > 0)
        {
            Build(new SegmentRange(0, 0, _leafCount - 1), initial);
        }
    }

    public void Update(int index, Element value)
    {
        ValidateIndex(index);
        Update(new SegmentRange(0, 0, _leafCount - 1), index, value);
    }

    public Element Query(int left, int right)
    {
        ValidateRange(left, right);
        return Query(new SegmentRange(0, 0, _leafCount - 1), left, right);
    }

    private void Build(SegmentRange range, IReadOnlyList<Element> initial)
    {
        if (range.Start == range.End)
        {
            _nodes.Set(range.Node, initial[range.Start]);
            return;
        }

        var (left, right) = range.Split();

        Build(left, initial);
        Build(right, initial);

        var combined = TOperation.Combine(_nodes.Get(left.Node), _nodes.Get(right.Node));
        _nodes.Set(range.Node, combined);
    }

    private void Update(SegmentRange range, int index, Element value)
    {
        if (range.Start == range.End)
        {
            _nodes.Set(range.Node, value);
            return;
        }

        var (left, right) = range.Split();

        if (index <= left.End)
        {
            Update(left, index, value);
        }
        else
        {
            Update(right, index, value);
        }

        var combined = TOperation.Combine(_nodes.Get(left.Node), _nodes.Get(right.Node));
        _nodes.Set(range.Node, combined);
    }

    private Element Query(SegmentRange range, int left, int right)
    {
        if (right < range.Start || range.End < left)
        {
            return TOperation.Identity;
        }

        if (left <= range.Start && range.End <= right)
        {
            return _nodes.Get(range.Node);
        }

        return QueryChildren(range, left, right);
    }

    // The range straddles both halves, so each child answers its own overlap and the
    // two answers combine.
    private Element QueryChildren(SegmentRange range, int left, int right)
    {
        var (leftChild, rightChild) = range.Split();
        var leftResult = Query(leftChild, left, right);
        var rightResult = Query(rightChild, left, right);

        return TOperation.Combine(leftResult, rightResult);
    }

    private void ValidateIndex(int index) => RangeBounds.ValidateIndex(index, _leafCount, IndexOutOfBoundsMessage);

    private void ValidateRange(int left, int right) => RangeBounds.ValidateRange(left, right, _leafCount, RangeOutOfBoundsMessage);
}
