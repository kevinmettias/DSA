using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Count, int Value)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// RLE Iterator (LC 900): the canonical approach eagerly decompresses the whole
// run-length encoding into a flat int[] and walks it with an index cursor - O(total
// element count) time and memory up front, which on LeetCode's own constraints
// (individual run counts up to 1e9) can be far larger than the encoding itself.
// The alternative consumes runs lazily from this repo's own FIFO Queue<(int,int)>,
// tracking only the currently-in-progress run's remaining count as instance state -
// O(number of runs) memory and O(total Next-calls + number of runs) time, since
// each run is dequeued at most once across the whole query stream.
[MemoryDiagnoser]
public class RLEIteratorBenchmarks
{
    private const int RunCount = 100;
    private const int RandomSeed = 900;
    private const int ValuesPerRun = 2;
    private const int MaxRunValueExclusive = 1_000;
    private const int MaxQueryLength = 50;

    [Params(2_000, 200_000)]
    public int TotalCount;

    private int[] _encoding = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _encoding = BuildEncoding(random, TotalCount);
        _queries = BuildQueries(random, TotalCount);
    }

    [Benchmark(Baseline = true)]
    public int DecompressedArrayCursor()
    {
        var values = Decompress(_encoding);
        var index = 0;
        var last = -1;

        foreach (var n in _queries)
        {
            var remaining = n;

            while (remaining > 0 && index < values.Length)
            {
                last = values[index++];
                remaining--;
            }

            if (remaining > 0)
            {
                last = -1;
            }
        }

        return last;
    }

    [Benchmark]
    public int RunLengthQueue()
    {
        var iterator = new RleIterator(_encoding);
        var last = -1;

        foreach (var n in _queries)
        {
            last = iterator.Next(n);
        }

        return last;
    }

    private static int[] BuildEncoding(Random random, int totalCount)
    {
        var encoding = new int[RunCount * ValuesPerRun];
        var remaining = totalCount;

        for (var run = 0; run < RunCount; run++)
        {
            var runsLeft = RunCount - run;
            var count = run == RunCount - 1 ? remaining : Math.Max(1, remaining / runsLeft);
            remaining -= count;

            encoding[run * ValuesPerRun] = count;
            encoding[(run * ValuesPerRun) + 1] = random.Next(1, MaxRunValueExclusive);
        }

        return encoding;
    }

    private static int[] BuildQueries(Random random, int totalCount)
    {
        var queries = new List<int>();
        var remaining = totalCount;

        while (remaining > 0)
        {
            var candidateLength = random.Next(1, MaxQueryLength);
            var n = Math.Min(remaining, candidateLength);
            queries.Add(n);
            remaining -= n;
        }

        return [.. queries];
    }

    private static int[] Decompress(int[] encoding)
    {
        var totalCount = 0;
        for (var i = 0; i < encoding.Length; i += ValuesPerRun)
        {
            totalCount += encoding[i];
        }

        var values = new int[totalCount];
        var index = 0;

        for (var i = 0; i < encoding.Length; i += ValuesPerRun)
        {
            for (var repeat = 0; repeat < encoding[i]; repeat++)
            {
                values[index++] = encoding[i + 1];
            }
        }

        return values;
    }

    private sealed class RleIterator
    {
        private readonly RepoQueue _runs = new();
        private int _remaining;
        private int _value;

        public RleIterator(int[] encoding)
        {
            for (var i = 0; i < encoding.Length; i += ValuesPerRun)
            {
                _runs.Enqueue((encoding[i], encoding[i + 1]));
            }
        }

        public int Next(int n)
        {
            while (n > 0)
            {
                if (_remaining == 0)
                {
                    if (!_runs.TryDequeue(out var run))
                    {
                        return -1;
                    }

                    _remaining = run.Count;
                    _value = run.Value;
                }

                var consumed = Math.Min(n, _remaining);
                _remaining -= consumed;
                n -= consumed;
            }

            return _value;
        }
    }
}
