namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// No new Topology witness, and no Representation change beyond reusing
// BinaryTreeNode<TValue> as-is (phase 1). BST ordering looks like Heap's
// Min-vs-Max axis (a re-derived-every-step comparison deciding control flow,
// §5's table row 1) but fails the sharper test §10.1 already gives for exactly
// this trap: does the choice change the observable OUTPUT, or only how it's
// reached? There is no second, library-enumerated "kind" of BST the way
// MaxHeapOrder is a real alternative to MinHeapOrder (which changes which value
// Pop() returns) - a "descending" BST is the same structure under a reversed
// IComparer<TValue>, the identical reversal move IComparer<T> already supports
// for BinarySearch/HashMap. So ordering stays a plain runtime IComparer<TValue>
// field, the same open bucket §9.2 already puts BinarySearch's comparer in - not
// a new IBstOrder<TValue> witness. The BST-ordered invariant itself needs no
// witness either, for the same reason DisjointSet's equivalence-relation
// invariant (§10.2) and HashMap's "at most one value per key" (§4.1) don't: a
// correct Insert/TryDelete cannot produce a non-BST-ordered tree through this
// type's own public surface, and no caller input can violate it (unlike
// BinarySearch's sortedness, which a caller does control and can violate).
//
// Insert/TryDelete/Has are hardwired directly against BinaryTreeNode<TValue>.
// Left/Right, not generic over IChildren/ITreeTopology - the identical reasoning
// InOrderTraversal's own doc comment already gives (a compacted IChildren view
// throws away exactly the left-vs-right positional identity routing a
// comparison result into the correct child slot needs). Per ARCHITECTURE.md §5
// step 7/§13.5 that means these co-locate under DataStructures/BinaryTree/, not
// Algorithms/ - InOrderTraversal.cs's own placement is the direct precedent.
//
// Plain, non-self-balancing BST: height is O(log n) for balanced insertion order
// but degrades to O(n) for adversarial order (e.g. ascending input), and
// recursive TryDelete risks a StackOverflowException at that depth on large,
// unbalanced inputs - an unenforced Representation/Complexity law, the same
// shape as HeapArray's O(1)-Get assumption (§8) or BinarySearch's sortedness
// precondition (§9.1). Self-balancing is out of scope here.
internal sealed class BinarySearchTree<TValue>
    where TValue : IComparable<TValue>
{
    private readonly IComparer<TValue> _comparer;
    private BinaryTreeNode<TValue>? _root;
    private int _count;

    public int Count => _count;

    public BinaryTreeNode<TValue>? Root => _root;

    public BinarySearchTree()
        : this(Comparer<TValue>.Default)
    {
    }

    public BinarySearchTree(IComparer<TValue> comparer) => _comparer = comparer;

    // Iterative, not recursive like TryDelete: BinaryTreeNode is a reference type, so
    // node.Left.Left = ... mutation is already visible through the held reference -
    // recursion here would add stack frames for zero benefit.
    public void Insert(TValue value)
    {
        if (_root is null)
        {
            _root = new BinaryTreeNode<TValue>(value);
            _count++;
            return;
        }

        var insertionPoint = FindInsertionPoint(_root, value);

        if (insertionPoint is { } point)
        {
            InsertChild(point.Parent, point.Side, value);
        }
    }

    // Null return means value already exists (duplicate, no-op) - the loop always
    // stops at either an equal node or an empty child slot.
    private (BinaryTreeNode<TValue> Parent, ChildSide Side)? FindInsertionPoint(
        BinaryTreeNode<TValue> root, TValue value)
    {
        var node = root;

        // Stops via one of the two returns below: an equal value found, or an
        // empty child slot reached - both unavoidable within a finite tree.
        while (true)
        {
            var comparison = _comparer.Compare(value, node.Value);

            if (comparison == 0)
            {
                return null;
            }

            var (side, next) = NextStep(node, comparison);

            if (next is null)
            {
                return (node, side);
            }

            node = next;
        }
    }

    private static (ChildSide Side, BinaryTreeNode<TValue>? Next) NextStep(BinaryTreeNode<TValue> node, int comparison)
    {
        var side = comparison < 0 ? ChildSide.Left : ChildSide.Right;
        return (side, side == ChildSide.Left ? node.Left : node.Right);
    }

    private void InsertChild(BinaryTreeNode<TValue> parent, ChildSide side, TValue value)
    {
        var child = new BinaryTreeNode<TValue>(value);

        if (side == ChildSide.Left)
        {
            parent.Left = child;
        }
        else
        {
            parent.Right = child;
        }

        _count++;
    }

    public bool Has(TValue value)
    {
        var node = _root;

        while (node is not null)
        {
            var comparison = _comparer.Compare(value, node.Value);

            if (comparison == 0)
            {
                return true;
            }

            node = comparison < 0 ? node.Left : node.Right;
        }

        return false;
    }

    // Check-then-act rather than threading a found/not-found result out of the
    // recursion - the same "redundant lookup for clarity" shape Set.TryAdd already
    // uses (HasKey then Set).
    public bool TryDelete(TValue value)
    {
        if (!Has(value))
        {
            return false;
        }

        _root = DeleteRecursive(_root, value);
        _count--;
        return true;
    }

    // Recursive: the "return the replacement subtree root, caller reassigns
    // node.Left/node.Right" shape is what makes root-replacement and re-parenting
    // fall out for free with no manual parent pointers - same convention TreeFold/
    // LowestCommonAncestor/InOrderTraversal already use.
    private BinaryTreeNode<TValue>? DeleteRecursive(BinaryTreeNode<TValue>? node, TValue value)
    {
        if (node is null)
        {
            return null;
        }

        var comparison = _comparer.Compare(value, node.Value);

        if (comparison < 0)
        {
            node.Left = DeleteRecursive(node.Left, value);
            return node;
        }

        if (comparison > 0)
        {
            node.Right = DeleteRecursive(node.Right, value);
            return node;
        }

        return DeleteFoundNode(node);
    }

    // The found node: leaf/right-only collapse to "return the surviving child"
    // (null for a leaf); left-only mirrors it. Two children promote the in-order
    // successor's value (the minimum of the right subtree, which by construction
    // has no left child, so removing it always lands in the leaf/one-child case
    // above and the recursion terminates in exactly one more step).
    private BinaryTreeNode<TValue>? DeleteFoundNode(BinaryTreeNode<TValue> node)
    {
        if (node.Left is null)
        {
            return node.Right;
        }

        if (node.Right is null)
        {
            return node.Left;
        }

        var successor = FindMin(node.Right);
        node.Value = successor.Value;
        node.Right = DeleteRecursive(node.Right, successor.Value);
        return node;
    }

    private static BinaryTreeNode<TValue> FindMin(BinaryTreeNode<TValue> node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }
}
