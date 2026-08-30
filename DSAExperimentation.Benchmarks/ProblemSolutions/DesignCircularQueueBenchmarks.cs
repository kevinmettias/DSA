using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Circular Queue (LC 622): the textbook fixed int[] + head/tail/count
// wraparound implementation vs. this repo's Deque<int> (already a wraparound-array
// Representation, ARCHITECTURE.md §4.1) wrapped with an external capacity bound for
// IsFull. Each [Benchmark] churns EnQueue/DeQueue pairs at a queue already at
// capacity, forcing every operation through the wraparound path at both ends.
[MemoryDiagnoser]
public class DesignCircularQueueBenchmarks
{
    private const int OperationCount = 50_000;

    [Params(8, 512)]
    public int Capacity;

    [Benchmark(Baseline = true)]
    public int ArrayBacked()
    {
        var queue = new ArrayCircularQueue(Capacity);
        var rearSum = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            if (queue.IsFull())
            {
                queue.DeQueue();
            }

            queue.EnQueue(i);
            rearSum += queue.Rear();
        }

        return rearSum;
    }

    [Benchmark]
    public int DequeBacked()
    {
        var queue = new DequeCircularQueue(Capacity);
        var rearSum = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            if (queue.IsFull())
            {
                queue.DeQueue();
            }

            queue.EnQueue(i);
            rearSum += queue.Rear();
        }

        return rearSum;
    }

    private sealed class ArrayCircularQueue
    {
        private readonly int[] _items;
        private int _head;
        private int _count;

        public ArrayCircularQueue(int capacity) => _items = new int[capacity];

        public bool IsFull() => _count == _items.Length;

        public void EnQueue(int value)
        {
            _items[(_head + _count) % _items.Length] = value;
            _count++;
        }

        public void DeQueue()
        {
            _head = (_head + 1) % _items.Length;
            _count--;
        }

        public int Rear() => _items[(_head + _count - 1 + _items.Length) % _items.Length];
    }

    private sealed class DequeCircularQueue
    {
        private readonly RepoDeque _items = new();
        private readonly int _capacity;

        public DequeCircularQueue(int capacity) => _capacity = capacity;

        public bool IsFull() => _items.Count == _capacity;

        public void EnQueue(int value) => _items.PushBack(value);

        public void DeQueue() => _items.TryPopFront(out _);

        public int Rear() => _items.TryPeekBack(out var value) ? value : -1;
    }
}
