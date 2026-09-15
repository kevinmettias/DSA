using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.DataStructures.LazySegmentTree;

// UpdateRange/Query are O(log n) amortized - a claim that assumes SegmentTreeArray<Element>.Get/Set
// are O(1). See ARCHITECTURE.md §8.
//
// TOperation.Combine must be associative and TOperation.ComposeUpdate must correctly fold any two
// consecutive pending updates into one equivalent update - both unenforced precondition laws, the
// same shape as BinarySearch's sortedness: getting either wrong still builds/updates/queries
// without error, just silently returns a wrong value once two updates land on the same node
// before it's pushed down.
//
// Reuses SegmentTree's own SegmentTreeArray<Element>/SegmentRange directly rather than declaring
// LazySegmentTree-local copies: composing another domain's already-public concrete Representation
// is the ARCHITECTURE.md §5 step 5 carve-out (what's prohibited is forcing that domain's type to
// implement *your* interface, not reusing its concrete type) - and unlike Heap/SegmentTree's
// merely-coincidental identical index arithmetic, SegmentTree and LazySegmentTree share this array
// representation *because* they are the same underlying array-as-complete-binary-tree layout, the
// same reason Stack composes DynamicArray rather than re-declaring it. A repo-wide structural-
// duplication check confirmed this the hard way: an earlier version of this file kept its own
// LazySegmentTreeArray/LazySegmentTreeIndex/LazySegmentRange copies, and check-duplicate-constant
// plus check-interfile-duplication both flagged the result against SegmentTree's identical files.
// IRangeUpdateOperation<Element,TUpdate> itself still stays independently declared, not inherited
// from ICombineOperation<Element> - that is a witness/Operations-law contract, exactly what §5
// step 5 *does* forbid reusing across domains (see IRangeUpdateOperation.cs's own doc comment).
//
// Range-update only, deliberately: no point-Update method. Mixing point-update semantics (which
// would need to bypass any pending tag) with this lazy range-update scheme on one instance would
// be a silent-wrong-answer trap - see SegmentTree, a separate structure for point-update/
// range-query.
internal sealed class LazySegmentTree<Element, TUpdate, TOperation>
    where TOperation : struct, IRangeUpdateOperation<Element, TUpdate>
{
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the lazy segment tree, or left exceeded right.";
    private const string NoUpdateSentinelMessage = "The update value equals TOperation.NoUpdate, which is reserved to mean \"nothing pending\" and cannot be applied as an explicit update - composing it into a node with an already-pending update would silently discard that update instead of preserving it.";

    private readonly SegmentTreeArray<Element> _values;
    private readonly SegmentTreeArray<TUpdate> _pending;
    private readonly int _leafCount;

    public int Count => _leafCount;

    public LazySegmentTree(IReadOnlyList<Element> initial)
    {
        _leafCount = initial.Count;
        _values = new SegmentTreeArray<Element>(_leafCount);
        _pending = new SegmentTreeArray<TUpdate>(_leafCount);

        if (_leafCount > 0)
        {
            Build(new SegmentRange(0, 0, _leafCount - 1), initial);
        }
    }

    public void UpdateRange(int left, int right, TUpdate update)
    {
        ValidateRange(left, right);
        ValidateUpdate(update);
        UpdateRange(new SegmentRange(0, 0, _leafCount - 1), left, right, update);
    }

    public Element Query(int left, int right)
    {
        ValidateRange(left, right);
        return Query(new SegmentRange(0, 0, _leafCount - 1), left, right);
    }

    // Also seeds _pending with NoUpdate at every node Build visits - the exact same node set
    // UpdateRange/Query/PushDown ever read via the identical [Start,End] partition recursion, so
    // no node is ever read before this initializes it.
    private void Build(SegmentRange range, IReadOnlyList<Element> initial)
    {
        _pending.Set(range.Node, TOperation.NoUpdate);

        if (range.Start == range.End)
        {
            _values.Set(range.Node, initial[range.Start]);
            return;
        }

        var (left, right) = range.Split();

        Build(left, initial);
        Build(right, initial);

        var combined = TOperation.Combine(_values.Get(left.Node), _values.Get(right.Node));
        _values.Set(range.Node, combined);
    }

    private void UpdateRange(SegmentRange range, int left, int right, TUpdate update)
    {
        if (TryResolveAtNode(range, left, right, update))
        {
            return;
        }

        PushDown(range);

        var (leftChild, rightChild) = range.Split();

        UpdateRange(leftChild, left, right, update);
        UpdateRange(rightChild, left, right, update);

        RecombineFromChildren(range, leftChild, rightChild);
    }

    // Rebuilds a node's own aggregate out of its two children - the step that makes a node's
    // value valid again once both of its children have finished absorbing the update.
    private void RecombineFromChildren(SegmentRange range, SegmentRange leftChild, SegmentRange rightChild)
    {
        var combined = TOperation.Combine(_values.Get(leftChild.Node), _values.Get(rightChild.Node));
        _values.Set(range.Node, combined);
    }

    // Settles the range at this node without descending when it can: a range disjoint from the
    // node needs nothing, and one that covers the node entirely is applied here outright. Anything
    // else has to reach the children, which is the caller's recursion.
    private bool TryResolveAtNode(SegmentRange range, int left, int right, TUpdate update)
    {
        if (right < range.Start || range.End < left)
        {
            return true;
        }

        if (left <= range.Start && range.End <= right)
        {
            ApplyToNode(range, update);
            return true;
        }

        return false;
    }

    private Element Query(SegmentRange range, int left, int right)
    {
        if (right < range.Start || range.End < left)
        {
            return TOperation.Identity;
        }

        if (left <= range.Start && range.End <= right)
        {
            return _values.Get(range.Node);
        }

        PushDown(range);

        var (leftChild, rightChild) = range.Split();
        var leftResult = Query(leftChild, left, right);
        var rightResult = Query(rightChild, left, right);

        return TOperation.Combine(leftResult, rightResult);
    }

    // Pushes this node's own pending update onto both children, then clears it - children absorb
    // it into both their own aggregate (ApplyUpdate) and their own pending tag (ComposeUpdate), so
    // a later push-down at their level replays the same update again.
    private void PushDown(SegmentRange range)
    {
        var pending = _pending.Get(range.Node);

        if (EqualityComparer<TUpdate>.Default.Equals(pending, TOperation.NoUpdate))
        {
            return;
        }

        ApplyToBothChildren(range, pending);

        _pending.Set(range.Node, TOperation.NoUpdate);
    }

    private void ApplyToBothChildren(SegmentRange range, TUpdate pending)
    {
        var (left, right) = range.Split();

        ApplyToNode(left, pending);
        ApplyToNode(right, pending);
    }

    private void ApplyToNode(SegmentRange range, TUpdate update)
    {
        var rangeLength = range.End - range.Start + 1;

        var appliedValue = TOperation.ApplyUpdate(_values.Get(range.Node), update, rangeLength);
        _values.Set(range.Node, appliedValue);

        var composedUpdate = TOperation.ComposeUpdate(update, _pending.Get(range.Node));
        _pending.Set(range.Node, composedUpdate);
    }

    private void ValidateRange(int left, int right) => RangeBounds.ValidateRange(left, right, _leafCount, RangeOutOfBoundsMessage);

    private static void ValidateUpdate(TUpdate update)
    {
        if (EqualityComparer<TUpdate>.Default.Equals(update, TOperation.NoUpdate))
        {
            throw new ArgumentException(NoUpdateSentinelMessage, nameof(update));
        }
    }
}
