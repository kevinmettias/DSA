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
        var index = 0;
        var havePeeked = false;
        var peeked = 0;
        var sum = 0L;

        bool HasNext() => havePeeked || index < _values.Length;

        int Peek()
        {
            if (!havePeeked)
            {
                peeked = _values[index++];
                havePeeked = true;
            }

            return peeked;
        }

        int Next()
        {
            var value = Peek();
            havePeeked = false;
            return value;
        }

        while (HasNext())
        {
            sum += Peek();
            sum += Next();
        }

        return sum;
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
