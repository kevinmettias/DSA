using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.DesignLinkedList;

// LeetCode 707. Design Linked List: Get/AddAtHead/AddAtTail/AddAtIndex/DeleteAtIndex
// over a 0-indexed list.
//
// This is a design problem - LeetCode's own shape is a stateful object with five
// operations, not a single return value - so the strategy choice is which
// representation backs it, the same CreateBy<Strategy> factory shape
// LRUCacheSolution/MinStackSolution/AllOneDataStructureSolution use for their own
// design problems.
//
// CreateBySinglyLinkedListChain walks this repo's own SinglyLinkedListNode<TValue>
// chain from a dummy-head sentinel - the same "dummy-head list building" precedent
// AddTwoNumbers already uses - so AddAtHead/AddAtIndex(0, _)/DeleteAtIndex(0) are
// branch-free special cases instead of needing their own "is this the first node"
// checks; every other index just walks .Next from the sentinel to the node just
// before it. This is the original inline test's private MyLinkedList, unchanged.
//
// CreateByArrayList is the textbook baseline this composition has to justify itself
// against: a BCL List<int>, where AddAtHead/AddAtIndex near the front are O(n)
// because every existing element must shift right - exactly the cost the original
// benchmark's ArrayListInsertAtFront arm measured directly, but only for AddAtHead in
// isolation. Promoted here to the full IMyLinkedList surface so it gets the same
// example coverage as the composed strategy.
internal static class DesignLinkedListSolution
{
    public static IMyLinkedList CreateBySinglyLinkedListChain() => new SinglyLinkedListChainMyLinkedList();

    public static IMyLinkedList CreateByArrayList() => new ArrayListMyLinkedList();

    internal interface IMyLinkedList
    {
        int Get(int index);

        void AddAtHead(int value);

        void AddAtTail(int value);

        void AddAtIndex(int index, int value);

        void DeleteAtIndex(int index);
    }

    private sealed class SinglyLinkedListChainMyLinkedList : IMyLinkedList
    {
        private readonly SinglyLinkedListNode<int> _dummyHead = new(default);
        private int _count;

        public int Get(int index)
        {
            if (index < 0 || index >= _count)
            {
                return -1;
            }

            return NodeBefore(index).Next!.Value;
        }

        public void AddAtHead(int value) => AddAtIndex(0, value);

        public void AddAtTail(int value) => AddAtIndex(_count, value);

        public void AddAtIndex(int index, int value)
        {
            if (index > _count)
            {
                return;
            }

            var startIndex = Math.Max(index, 0);
            var previous = NodeBefore(startIndex);
            var node = new SinglyLinkedListNode<int>(value) { Next = previous.Next };
            previous.Next = node;
            _count++;
        }

        public void DeleteAtIndex(int index)
        {
            if (index < 0 || index >= _count)
            {
                return;
            }

            var previous = NodeBefore(index);
            previous.Next = previous.Next!.Next;
            _count--;
        }

        private SinglyLinkedListNode<int> NodeBefore(int index)
        {
            var node = _dummyHead;
            for (var i = 0; i < index; i++)
            {
                node = node.Next!;
            }

            return node;
        }
    }

    private sealed class ArrayListMyLinkedList : IMyLinkedList
    {
        private readonly List<int> _values = [];

        public int Get(int index) => index < 0 || index >= _values.Count ? -1 : _values[index];

        public void AddAtHead(int value) => AddAtIndex(0, value);

        public void AddAtTail(int value) => AddAtIndex(_values.Count, value);

        public void AddAtIndex(int index, int value)
        {
            if (index > _values.Count)
            {
                return;
            }

            _values.Insert(Math.Max(index, 0), value);
        }

        public void DeleteAtIndex(int index)
        {
            if (index < 0 || index >= _values.Count)
            {
                return;
            }

            _values.RemoveAt(index);
        }
    }
}
