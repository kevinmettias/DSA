using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Front Middle Back Queue (LC 1670): a single List<int> reaching for
// Insert(0,_)/Insert(Count/2,_) for front/middle pushes (each an O(n) shift of every
// following element) vs. this repo's two-Deque<int> split, where a push at any of the
// three positions is O(1) (amortized, across the occasional single-element rebalance
// move between the two deques) - the same DesignLinkedListBenchmarks precedent
// (array-shift baseline vs. O(1) primitive-based insert), just with a third position.
[MemoryDiagnoser]
public class DesignFrontMiddleBackQueueBenchmarks
{
    [Params(5_000, 50_000)]
    public int Calls;

    [Benchmark(Baseline = true)]
    public int ArrayListInsertAtPosition()
    {
        var list = new List<int>();

        for (var i = 0; i < Calls; i++)
        {
            switch (i % 3)
            {
                case 0: list.Insert(0, i); break;
                case 1: list.Insert(list.Count / 2, i); break;
                default: list.Add(i); break;
            }
        }

        return list.Count;
    }

    [Benchmark]
    public int TwoDequeFrontMiddleBackQueue()
    {
        var queue = new FrontMiddleBackQueue();

        for (var i = 0; i < Calls; i++)
        {
            switch (i % 3)
            {
                case 0: queue.PushFront(i); break;
                case 1: queue.PushMiddle(i); break;
                default: queue.PushBack(i); break;
            }
        }

        return queue.Count;
    }

    private sealed class FrontMiddleBackQueue
    {
        private readonly RepoDeque _front = new();
        private readonly RepoDeque _back = new();

        public int Count => _front.Count + _back.Count;

        public void PushFront(int value)
        {
            _front.PushFront(value);
            Rebalance();
        }

        public void PushMiddle(int value)
        {
            if (_front.Count < _back.Count)
            {
                _front.PushBack(value);
            }
            else
            {
                _back.PushFront(value);
            }
        }

        public void PushBack(int value)
        {
            _back.PushBack(value);
            Rebalance();
        }

        private void Rebalance()
        {
            if (_front.Count > _back.Count + 1)
            {
                _front.TryPopBack(out var value);
                _back.PushFront(value);
            }
            else if (_back.Count > _front.Count + 1)
            {
                _back.TryPopFront(out var value);
                _front.PushBack(value);
            }
        }
    }
}
