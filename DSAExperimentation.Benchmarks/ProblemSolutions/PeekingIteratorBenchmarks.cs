using BenchmarkDotNet.Attributes;
using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Peeking Iterator (LC 284): a hand-rolled index + "have I already peeked" flag over
// the raw array vs. this repo's own Queue<T>, whose TryPeek/TryDequeue already are
// exactly the "look without consuming" / "look and consume" pair the problem asks
// for - no extra buffering state to write by hand. Both drive the same
// peek-peek-next pattern to full exhaustion.
[MemoryDiagnoser]
public class PeekingIteratorBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public long IndexTrackedPeek()
    {
        var iterator = new IndexTrackedIterator(_values);
        var sum = 0L;

        while (iterator.HasNext())
        {
            sum += iterator.Peek();
            sum += iterator.Next();
        }

        return sum;
    }

    private sealed class IndexTrackedIterator(int[] values)
    {
        private int _index;
        private bool _havePeeked;
        private int _peeked;

        public bool HasNext() => _havePeeked || _index < values.Length;

        public int Peek()
        {
            if (!_havePeeked)
            {
                _peeked = values[_index++];
                _havePeeked = true;
            }

            return _peeked;
        }

        public int Next()
        {
            var value = Peek();
            _havePeeked = false;
            return value;
        }
    }

    [Benchmark]
    public long QueueBackedPeek()
    {
        var items = new RepoIntQueue();

        foreach (var value in _values)
        {
            items.Enqueue(value);
        }

        var sum = 0L;

        while (items.Count > 0)
        {
            items.TryPeek(out var peeked);
            sum += peeked;
            items.TryDequeue(out var next);
            sum += next;
        }

        return sum;
    }
}
