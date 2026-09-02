using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Circular Deque (LC 641): the textbook fixed int[] + head/count wraparound
// implementation, doing the wraparound arithmetic by hand at both ends, vs. this
// repo's Deque<int> (already a wraparound-array Representation, ARCHITECTURE.md
// §4.1) wrapped with an external capacity bound for IsFull - the same comparison
// DesignCircularQueueBenchmarks already makes for Design Circular Queue (LC 622),
// extended to exercise InsertFront/DeleteLast too, not just the FIFO-only end pair,
// since this problem allows both ends. Each [Benchmark] churns an insert-last/
// insert-front/delete-front/delete-last cycle that settles into a steady state at
// (near) capacity, forcing every operation through the wraparound path on both ends.
[MemoryDiagnoser]
public class DesignCircularDequeBenchmarks
{
    private const int OperationCount = 50_000;

    [Params(8, 512)]
    public int Capacity;

    [Benchmark(Baseline = true)]
    public int ArrayBacked() => RunChurnCycle(new ArrayCircularDeque(Capacity));

    [Benchmark]
    public int DequeBacked() => RunChurnCycle(new DequeCircularDeque(Capacity));

    private static int RunChurnCycle(ICircularDeque deque)
    {
        var sum = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            if (deque.IsFull())
            {
                deque.DeleteFront();
            }

            deque.InsertLast(i);

            if (deque.IsFull())
            {
                deque.DeleteLast();
            }

            deque.InsertFront(i);
            sum += deque.GetFront() + deque.GetRear();
        }

        return sum;
    }

    private interface ICircularDeque
    {
        bool IsFull();

        void InsertFront(int value);

        void InsertLast(int value);

        void DeleteFront();

        void DeleteLast();

        int GetFront();

        int GetRear();
    }

    private sealed class ArrayCircularDeque(int capacity) : ICircularDeque
    {
        private readonly int[] _items = new int[capacity];
        private int _head;
        private int _count;

        public bool IsFull() => _count == _items.Length;

        public void InsertFront(int value)
        {
            _head = (_head - 1 + _items.Length) % _items.Length;
            _items[_head] = value;
            _count++;
        }

        public void InsertLast(int value)
        {
            _items[(_head + _count) % _items.Length] = value;
            _count++;
        }

        public void DeleteFront()
        {
            _head = (_head + 1) % _items.Length;
            _count--;
        }

        public void DeleteLast() => _count--;

        public int GetFront() => _items[_head];

        public int GetRear() => _items[(_head + _count - 1 + _items.Length) % _items.Length];
    }

    private sealed class DequeCircularDeque(int capacity) : ICircularDeque
    {
        private readonly RepoDeque _items = new();

        public bool IsFull() => _items.Count == capacity;

        public void InsertFront(int value) => _items.PushFront(value);

        public void InsertLast(int value) => _items.PushBack(value);

        public void DeleteFront() => _items.TryPopFront(out _);

        public void DeleteLast() => _items.TryPopBack(out _);

        public int GetFront() => _items.TryPeekFront(out var value) ? value : -1;

        public int GetRear() => _items.TryPeekBack(out var value) ? value : -1;
    }
}
