using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.IntervalSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Module (LC 715): queryRange is the operation worth benchmarking - addRange/
// removeRange cost is dominated by the same O(n) array shift either representation
// pays (a flat List or IntervalSet's own DynamicArray-backed storage), so both
// versions are pre-populated with the same Length disjoint ranges once, up front,
// and only queryRange's own lookup cost is measured. LinearScan checks every stored
// range in a flat List<(int,int)> for full containment, O(n) per query.
// IntervalSetBinarySearch instead composes this repo's own IntervalSet<int> with
// BinarySearch.UpperBound over a Starts view to find the one candidate range that
// could contain the query, O(log n) per query - the same "IRandomAccessSequence
// witness over an existing structure's own Get" idiom IntervalSet.cs's own
// HasOverlap/Add already use internally.
[MemoryDiagnoser]
public class RangeModuleBenchmarks
{
    private const int RangeWidth = 2;
    private const int Stride = 4;
    private const int RandomSeed = 17;

    [Params(200, 5_000)]
    public int Length;

    private List<(int Start, int End)> _linearRanges = null!;
    private IntervalSet<int> _intervalSet = null!;
    private (int Left, int Right)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        _linearRanges = [];
        _intervalSet = new IntervalSet<int>();

        for (var i = 0; i < Length; i++)
        {
            var start = i * Stride;
            var end = start + RangeWidth;
            _linearRanges.Add((start, end));
            _intervalSet.Add(start, end);
        }

        var random = new Random(RandomSeed);
        var span = Length * Stride;
        _queries = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var left = random.Next(0, span);
                return (left, left + 1);
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var trueCount = 0;

        foreach (var (left, right) in _queries)
        {
            if (_linearRanges.Any(range => range.Start <= left && right <= range.End))
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    [Benchmark]
    public int IntervalSetBinarySearch()
    {
        var trueCount = 0;

        foreach (var (left, right) in _queries)
        {
            var candidate = BinarySearch.UpperBound<int, StartsView>(new StartsView(_intervalSet), left) - 1;
            if (candidate >= 0 && _intervalSet.Get(candidate).End >= right)
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private readonly struct StartsView(IntervalSet<int> intervals) : IRandomAccessSequence<int>
    {
        public int Length => intervals.Count;

        public int Get(int index) => intervals.Get(index).Start;
    }
}
