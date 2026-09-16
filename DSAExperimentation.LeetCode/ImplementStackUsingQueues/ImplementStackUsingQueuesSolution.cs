using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;
using BclQueue = System.Collections.Generic.Queue<int>;

namespace DSAExperimentation.LeetCode.ImplementStackUsingQueues;

// LeetCode 225. Implement Stack using Queues: a stack whose Push/Pop/Top/Empty
// are built only from queue-native operations (enqueue/dequeue/size) - no
// direct random access to the underlying storage.
//
// This is a design problem - LeetCode's own shape is a stateful object with
// four operations, not a single return value - so each strategy is a factory
// handing back the shared IStackOperations surface below, the same shape
// MinStackSolution/LRUCacheSolution use for their own design problems. Both
// arms make Push the costly operation: enqueue the new element, then rotate
// every earlier element behind it so it dequeues first; Pop/Top/Empty are
// then plain O(1) queue calls with no algorithm left to differ on. The only
// thing that differs is which queue backs the rotation - the CLR's own
// Queue<int> for the baseline (the arm this repo's own Queue<int> has to
// justify itself against), this repo's own Queue<int> for the composed
// answer.
internal static class ImplementStackUsingQueuesSolution
{
    internal interface IStackOperations
    {
        void Push(int value);

        int Pop();

        int Top();

        bool Empty();
    }

    // The textbook answer: BCL Queue<int>, nothing from this repo.
    public static IStackOperations CreateByBuiltInQueue() => new BuiltInQueueStack();

    // This repo's own Queue<int> doing the identical rotate-on-push.
    public static IStackOperations CreateByQueuePrimitive() => new QueuePrimitiveStack();

    private sealed class BuiltInQueueStack : IStackOperations
    {
        private BclQueue _items = new();

        public void Push(int value)
        {
            var rotated = new BclQueue();
            rotated.Enqueue(value);

            while (_items.Count > 0)
            {
                rotated.Enqueue(_items.Dequeue());
            }

            _items = rotated;
        }

        public int Pop() => _items.Dequeue();

        public int Top() => _items.Peek();

        public bool Empty() => _items.Count == 0;
    }

    private sealed class QueuePrimitiveStack : IStackOperations
    {
        private RepoQueue _items = new();

        public void Push(int value)
        {
            var rotated = new RepoQueue();
            rotated.Enqueue(value);

            while (_items.TryDequeue(out var item))
            {
                rotated.Enqueue(item);
            }

            _items = rotated;
        }

        public int Pop()
        {
            _items.TryDequeue(out var x);
            return x;
        }

        public int Top()
        {
            _items.TryPeek(out var x);
            return x;
        }

        public bool Empty() => _items.Count == 0;
    }
}
