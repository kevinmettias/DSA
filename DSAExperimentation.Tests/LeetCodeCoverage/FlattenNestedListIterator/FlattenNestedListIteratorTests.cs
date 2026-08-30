using DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator.Fixtures;
using NestedStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator.Fixtures.NestedInteger>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator;

// LeetCode 341. Flatten Nested List Iterator: this repo's own LIFO Stack<T> holding
// not-yet-flattened NestedIntegers, pushed in reverse so the leftmost element pops
// first. HasNext lazily unwraps nested lists (pushing their children back on, still
// reversed) until the top is a plain integer or the stack empties - the same lazy
// "only do the work next() actually needs" shape BinarySearchTreeIteratorTests'
// PushLeft gives in-order BST traversal.
public sealed partial class FlattenNestedListIteratorTests
{
    [Fact]
    public void Next_ClassicExample_FlattensNestedListInOrder()
    {
        // [[1,1],2,[1,1]]
        List<NestedInteger> nestedList =
        [
            NestedInteger.OfList(NestedInteger.OfInteger(1), NestedInteger.OfInteger(1)),
            NestedInteger.OfInteger(2),
            NestedInteger.OfList(NestedInteger.OfInteger(1), NestedInteger.OfInteger(1)),
        ];

        Assert.Equal([1, 1, 2, 1, 1], Flatten(nestedList));
    }

    [Fact]
    public void Next_DeeplyNestedList_FlattensEveryDepthInOrder()
    {
        // [1,[4,[6]]]
        List<NestedInteger> nestedList =
        [
            NestedInteger.OfInteger(1),
            NestedInteger.OfList(NestedInteger.OfInteger(4), NestedInteger.OfList(NestedInteger.OfInteger(6))),
        ];

        Assert.Equal([1, 4, 6], Flatten(nestedList));
    }

    [Fact]
    public void HasNext_EmptyNestedListsInterspersed_SkipsThemEntirely()
    {
        // [[],1,[],2,[]]
        List<NestedInteger> nestedList =
        [
            NestedInteger.OfList(),
            NestedInteger.OfInteger(1),
            NestedInteger.OfList(),
            NestedInteger.OfInteger(2),
            NestedInteger.OfList(),
        ];

        Assert.Equal([1, 2], Flatten(nestedList));
    }

    private static List<int> Flatten(List<NestedInteger> nestedList)
    {
        var iterator = new NestedIterator(nestedList);
        var flattened = new List<int>();

        while (iterator.HasNext())
        {
            flattened.Add(iterator.Next());
        }

        return flattened;
    }

    private sealed class NestedIterator
    {
        private readonly NestedStack _pending = new();

        public NestedIterator(List<NestedInteger> nestedList) => PushReversed(nestedList);

        public bool HasNext()
        {
            while (_pending.TryPeek(out var top) && !top.IsInteger)
            {
                _pending.TryPop(out _);
                PushReversed(top.Elements);
            }

            return _pending.Count > 0;
        }

        public int Next()
        {
            _pending.TryPop(out var top);
            return top.Value;
        }

        private void PushReversed(List<NestedInteger> elements)
        {
            for (var i = elements.Count - 1; i >= 0; i--)
            {
                _pending.Push(elements[i]);
            }
        }
    }
}
