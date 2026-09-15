using NestedStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.LeetCode.FlattenNestedListIterator.NestedInteger>;

namespace DSAExperimentation.LeetCode.FlattenNestedListIterator;

// LeetCode 341. Flatten Nested List Iterator: HasNext()/Next() walks a
// [[1,1],2,[1,1]]-shaped nested structure in order.
//
// The two strategies differ in when they do the unwrapping work. CreateByLazyStack
// keeps this repo's own Stack<NestedInteger> of not-yet-flattened nodes, pushed in
// reverse so the leftmost pops first, and only unwraps a nested list once HasNext
// walks past it - the same lazy "only do the work Next() actually needs" shape
// BinarySearchTreeIteratorSolution's left-spine stack gives in-order BST traversal.
// CreateByEagerFlatten is the textbook alternative it is measured against: recurse
// over the whole structure up front into a BCL List<int> and just index through it,
// regardless of how much of it a caller actually consumes.
internal static class FlattenNestedListIteratorSolution
{
    public static IFlattenIterator CreateByLazyStack(List<NestedInteger> nestedList) =>
        new LazyStackIterator(nestedList);

    public static IFlattenIterator CreateByEagerFlatten(List<NestedInteger> nestedList) =>
        new EagerFlattenIterator(nestedList);

    // LeetCode's own hasNext()/next() contract, shared by both strategies so a
    // harness can drain either one the same way.
    internal interface IFlattenIterator
    {
        bool HasNext();

        int Next();
    }

    private sealed class LazyStackIterator : IFlattenIterator
    {
        private readonly NestedStack _pending = new();

        public LazyStackIterator(List<NestedInteger> nestedList) => PushReversed(nestedList);

        public bool HasNext()
        {
            while (_pending.TryPeek(out var top) && !top.IsInteger)
            {
                _pending.TryPop(out _);
                PushReversed(top.Elements);
            }

            return _pending.Count > 0;
        }

        private void PushReversed(List<NestedInteger> elements)
        {
            for (var i = elements.Count - 1; i >= 0; i--)
            {
                _pending.Push(elements[i]);
            }
        }

        public int Next()
        {
            _pending.TryPop(out var top);
            return top.Value;
        }
    }

    // Deliberately BCL past the constructor's own input: an ordinary recursive
    // walk into a List<int>, then index through it - the baseline the lazy stack
    // above has to justify itself against.
    private sealed class EagerFlattenIterator : IFlattenIterator
    {
        private readonly List<int> _flattened = [];
        private int _index;

        public EagerFlattenIterator(List<NestedInteger> nestedList) => FlattenInto(nestedList, _flattened);

        public bool HasNext() => _index < _flattened.Count;

        public int Next() => _flattened[_index++];

        private static void FlattenInto(List<NestedInteger> nestedList, List<int> destination)
        {
            foreach (var element in nestedList)
            {
                if (element.IsInteger)
                {
                    destination.Add(element.Value);
                }
                else
                {
                    FlattenInto(element.Elements, destination);
                }
            }
        }
    }
}
