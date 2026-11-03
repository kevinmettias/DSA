using System.Runtime.CompilerServices;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Algorithms.Folding;

// The child results of every node a recursive fold has open, on one array. A node pushes each
// child's result as that child finishes, hands Combine the span of its own, and pops them before it
// returns. The order is a stack's because a child's whole subtree is folded, and popped, before the
// child's own result is pushed, so a node's results always sit at the top, contiguous. One array
// serves the whole fold where an array per node used to (HouseRobberIII measured that allocation at
// four to five times the hand recursion it stands for).
//
// A span into the array is valid only until the next push, which may grow it: Combine runs between
// the pushes and the pop, and an algebra's Combine is pure, so it never outlives its own call.
internal sealed class FoldStack<TResult>
{
    private TResult[] _items = new TResult[ArrayGrowth.InitialCapacity];
    private int _count;

    // Where the results of the node about to fold its children begin.
    public int Top => _count;

    public void Push(TResult result)
    {
        if (_count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * ArrayGrowth.GrowthFactor);
        }

        _items[_count] = result;
        _count++;
    }

    // The results pushed since start: one node's children, in the order they were folded.
    public ReadOnlySpan<TResult> Since(int start) => _items.AsSpan(start, _count - start);

    // Drops the results pushed since start, clearing any references they hold so the array keeps
    // nothing alive past the node that used them.
    public void PopTo(int start)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<TResult>())
        {
            Array.Clear(_items, start, _count - start);
        }

        _count = start;
    }
}
