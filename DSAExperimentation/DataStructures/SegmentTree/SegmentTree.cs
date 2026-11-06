using System.Runtime.CompilerServices;
using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.DataStructures.SegmentTree;

// The one segment tree (ARCHITECTURE.md §13.10). Every node holds TOperation's combination
// of the leaves it covers, and Build/Update/Query recurse over SegmentRange.Split. The
// configurations differ only in TAction, how an update to a whole range acts on those combinations:
// - SegmentTree<Element, TOperation> injects NoAction, the trivial action: point updates and range
//   queries.
// - SegmentTree<Element, TUpdate, TOperation> injects TOperation's own action, and adds
//   UpdateRange.
//
// Propagation is written once, here, and a configuration without an action compiles it out. Its
// update type is NoUpdate, so the constructor allocates no pending tags and every push-down sits
// behind a test of that type, which the JIT folds for each instantiation, even in code shared by
// reference-type elements. A static abstract on NoAction would not fold there: NoAction<Element>
// over a reference type is shared code itself, and the JIT calls its members through a runtime
// lookup. The test sits in one always-inlined helper, ChildrenOf, which says why.
//
// The monoid is a type argument beside the action rather than reached through it, for the same
// reason: the tree calls TOperation.Combine directly, as the point tree always did. A NoAction that
// forwarded the monoid would be one more call per combine for a reference-type element.
//
// Abstract, with a constructor only this assembly can call, so the two configurations are the whole
// set. UpdateRange is then a member only the configuration with an action declares, not one the
// trivial action would have to refuse at run time (the rule §20 gives for an operation a
// configuration lacks). Deriving rather than wrapping keeps each configuration one object from its
// arrays.
//
// TOperation.Combine must be associative and TAction.ComposeUpdate must fold any two consecutive
// pending updates into one equivalent update - unenforced precondition laws, the same shape as
// BinarySearch's sortedness: breaking either still builds, updates and queries without error, and
// silently answers wrong. Update and Query are O(log n), and UpdateRange O(log n) amortized, claims
// that assume SegmentTreeArray<Element>.Get/Set are O(1) (ARCHITECTURE.md §8).
/// <summary>
/// A fixed-length sequence with point updates and range queries, configured by how range updates
/// act on it.
/// </summary>
/// <typeparam name="Element">The element type.</typeparam>
/// <typeparam name="TUpdate">
/// The type of one range update, <see cref="NoUpdate"/> when there are none.
/// </typeparam>
/// <typeparam name="TOperation">The monoid every node combines its children with.</typeparam>
/// <typeparam name="TAction">How a range update acts on combined values.</typeparam>
public abstract class SegmentTree<Element, TUpdate, TOperation, TAction>
    where TOperation : struct, IMonoid<Element>
    where TAction : struct, IRangeAction<Element, TUpdate>
{
    private const string IndexOutOfBoundsMessage = "Index was outside the bounds of the segment tree.";
    private const string RangeOutOfBoundsMessage = "Range was outside the bounds of the segment tree, or left exceeded right.";

    private readonly SegmentTreeArray<Element> _values;

    // One tag per node while an update waits to reach its children, or null under the trivial
    // action, which never defers anything.
    private readonly SegmentTreeArray<TUpdate>? _pending;

    private readonly int _leafCount;

    /// <summary>The number of elements, fixed at construction.</summary>
    public int Count => _leafCount;

    // The arena that starts every pending tag as TAction.NoUpdate is filled at construction, so
    // every node reads as "nothing pending" before Build touches anything, not merely the nodes it
    // visits.
    private protected SegmentTree(IReadOnlyList<Element> initial)
    {
        _leafCount = initial.Count;
        _values = new SegmentTreeArray<Element>(_leafCount);

        if (typeof(TUpdate) != typeof(NoUpdate))
        {
            _pending = new SegmentTreeArray<TUpdate>(_leafCount, TAction.NoUpdate);
        }

        if (_leafCount > 0)
        {
            Build(new SegmentRange(0, 0, _leafCount - 1), initial);
        }
    }

    /// <summary>Replaces the element at an index.</summary>
    /// <param name="index">The 0-based index of the element to replace.</param>
    /// <param name="value">The element's new value.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is outside [0, Count).
    /// </exception>
    public void Update(int index, Element value)
    {
        ValidateIndex(index);
        Update(new SegmentRange(0, 0, _leafCount - 1), index, value);
    }

    /// <summary>Combines the elements of the inclusive range [left, right].</summary>
    /// <param name="left">The 0-based index of the range's first element.</param>
    /// <param name="right">The 0-based index of the range's last element.</param>
    /// <returns>
    /// The elements' combined value in index order, reflecting every update applied so far.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The range is reversed or reaches outside [0, Count).
    /// </exception>
    public Element Query(int left, int right)
    {
        ValidateRange(left, right);
        return Query(new SegmentRange(0, 0, _leafCount - 1), left, right);
    }

    /// <summary>
    /// Applies an update to every element of [left, right] that a node's range holds.
    /// </summary>
    /// <param name="range">The node, and the range of elements it covers.</param>
    /// <param name="left">
    /// The 0-based index of the updated range's first element, already validated.
    /// </param>
    /// <param name="right">
    /// The 0-based index of the updated range's last element, already validated.
    /// </param>
    /// <param name="update">The update, which must not be TAction.NoUpdate.</param>
    private protected void ApplyToRange(SegmentRange range, int left, int right, TUpdate update)
    {
        if (TryResolveAtNode(range, left, right, update))
        {
            return;
        }

        PushDown(range);

        var (leftChild, rightChild) = range.Split();

        ApplyToRange(leftChild, left, right, update);
        ApplyToRange(rightChild, left, right, update);

        RecombineFromChildren(range, leftChild, rightChild);
    }

    /// <summary>Throws unless [left, right] is a range of elements, in order.</summary>
    /// <param name="left">The 0-based index of the range's first element.</param>
    /// <param name="right">The 0-based index of the range's last element.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The range is reversed or reaches outside [0, Count).
    /// </exception>
    private protected void ValidateRange(int left, int right) =>
        RangeBounds.ValidateRange(left, right, _leafCount, RangeOutOfBoundsMessage);

    private void Build(SegmentRange range, IReadOnlyList<Element> initial) =>
        SegmentTreeBuild.Fill<Element, TOperation>(_values, range, initial);

    private void Update(SegmentRange range, int index, Element value)
    {
        if (range.Start == range.End)
        {
            _values.Set(range.Node, value);
            return;
        }

        // A tag pending here covers the leaf being replaced, so it reaches both children before one
        // of them is rewritten and the two recombine.
        var (left, right) = ChildrenOf(in range);

        if (index <= left.End)
        {
            Update(left, index, value);
        }
        else
        {
            Update(right, index, value);
        }

        var combined = TOperation.Combine(_values.Get(left.Node), _values.Get(right.Node));
        _values.Set(range.Node, combined);
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

        return QueryChildren(range, left, right);
    }

    // The range straddles both halves, so each child answers its own overlap and the two answers
    // combine, once any tag pending here has reached them.
    private Element QueryChildren(SegmentRange range, int left, int right)
    {
        var (leftChild, rightChild) = ChildrenOf(in range);
        var leftResult = Query(leftChild, left, right);
        var rightResult = Query(rightChild, left, right);

        return TOperation.Combine(leftResult, rightResult);
    }

    // A node's two children, once any tag pending on the node has reached them: what Update and
    // Query split into. The trivial action's tree keeps no tags, so the test folds away and this is
    // Split alone. It sits in this always-inlined helper rather than in the callers because a
    // branch written in a caller's own IL, though folded, changed the JIT's profile-guided
    // inlining of that caller (ARCHITECTURE.md §13.10).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (SegmentRange Left, SegmentRange Right) ChildrenOf(in SegmentRange range)
    {
        if (typeof(TUpdate) != typeof(NoUpdate))
        {
            PushDown(range);
        }

        return range.Split();
    }

    // Pushes this node's own pending update onto both children, then clears it - children absorb
    // it into both their own aggregate (ApplyUpdate) and their own pending tag (ComposeUpdate), so
    // a later push-down at their level replays the same update again.
    private void PushDown(SegmentRange range)
    {
        // presumption: allow -- only a configuration with an action reaches PushDown (ChildrenOf and
        // ApplyToRange), and its constructor always builds the tag arena.
        var pending = _pending!.Get(range.Node);

        if (EqualityComparer<TUpdate>.Default.Equals(pending, TAction.NoUpdate))
        {
            return;
        }

        ApplyToBothChildren(range, pending);

        _pending.Set(range.Node, TAction.NoUpdate);
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

        var appliedValue = TAction.ApplyUpdate(_values.Get(range.Node), update, rangeLength);
        _values.Set(range.Node, appliedValue);

        // presumption: allow -- ApplyToNode runs only under PushDown or ApplyToRange, which only a
        // configuration with an action reaches, and its constructor always builds the tag arena.
        var composedUpdate = TAction.ComposeUpdate(update, _pending!.Get(range.Node));
        _pending.Set(range.Node, composedUpdate);
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

    // Rebuilds a node's own aggregate out of its two children - the step that makes a node's
    // value valid again once both of its children have finished absorbing the update.
    private void RecombineFromChildren(
        SegmentRange range, SegmentRange leftChild, SegmentRange rightChild)
    {
        var combined = TOperation.Combine(_values.Get(leftChild.Node), _values.Get(rightChild.Node));
        _values.Set(range.Node, combined);
    }

    private void ValidateIndex(int index) =>
        RangeBounds.ValidateIndex(index, _leafCount, IndexOutOfBoundsMessage);
}

// The one tree with the trivial action injected (ARCHITECTURE.md §13.10): point updates and
// range queries, compiled to the same code the point-update tree had before the two trees became
// one, with no pending tags allocated and no push-down left in Update or Query. Derived rather than
// wrapped, so it is the tree itself, the way Deque is CircularBuffer itself (§20).
/// <summary>
/// A fixed-length sequence with point updates and range queries, each in O(log n).
/// </summary>
/// <typeparam name="Element">The element type.</typeparam>
/// <typeparam name="TOperation">The monoid a range query combines its elements with.</typeparam>
/// <param name="initial">The elements, in index order.</param>
public sealed class SegmentTree<Element, TOperation>(IReadOnlyList<Element> initial)
    : SegmentTree<Element, NoUpdate, TOperation, NoAction<Element>>(initial)
    where TOperation : struct, IMonoid<Element>;

// The one tree with TOperation's own action injected (ARCHITECTURE.md §13.10): what the
// separate LazySegmentTree was, now the same recursion as the point tree with propagation compiled
// in. It adds the one operation the trivial action has no use for, UpdateRange, and keeps the
// inherited point Update, which pushes any tag on its path down before it rewrites a leaf, so the
// two kinds of update mix safely on one instance.
/// <summary>
/// A fixed-length sequence with point and range updates and range queries, range updates in O(log
/// n) amortized.
/// </summary>
/// <typeparam name="Element">The element type.</typeparam>
/// <typeparam name="TUpdate">The type of one range update.</typeparam>
/// <typeparam name="TOperation">
/// The monoid a range query combines with, and the action a range update takes.
/// </typeparam>
/// <param name="initial">The elements, in index order.</param>
public sealed class SegmentTree<Element, TUpdate, TOperation>(IReadOnlyList<Element> initial)
    : SegmentTree<Element, TUpdate, TOperation, TOperation>(initial)
    where TOperation : struct, IRangeUpdateOperation<Element, TUpdate>
{
    private const string NoUpdateSentinelMessage = "The update value equals TOperation.NoUpdate, which is reserved to mean \"nothing pending\" and cannot be applied as an explicit update - composing it into a node with an already-pending update would silently discard that update instead of preserving it.";

    /// <summary>Applies an update to every element of the inclusive range [left, right].</summary>
    /// <param name="left">The 0-based index of the range's first element.</param>
    /// <param name="right">The 0-based index of the range's last element.</param>
    /// <param name="update">
    /// The update to apply, which must not be <c>TOperation.NoUpdate</c>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The range is reversed or reaches outside [0, Count).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="update"/> equals <c>TOperation.NoUpdate</c>.
    /// </exception>
    public void UpdateRange(int left, int right, TUpdate update)
    {
        ValidateRange(left, right);
        ValidateUpdate(update);
        ApplyToRange(new SegmentRange(0, 0, Count - 1), left, right, update);
    }

    private static void ValidateUpdate(TUpdate update)
    {
        if (EqualityComparer<TUpdate>.Default.Equals(update, TOperation.NoUpdate))
        {
            throw new ArgumentException(NoUpdateSentinelMessage, nameof(update));
        }
    }
}
