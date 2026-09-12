using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RangeModule;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Module (LC 715): harness only, both arms are RangeModuleSolution's, the same
// factories RangeModuleTests proves correct. queryRange is the operation worth
// benchmarking - addRange cost is dominated by the same O(n) array shift either
// representation pays (a flat List or IntervalSet's own DynamicArray-backed storage),
// so both instances are pre-populated with the same Length disjoint ranges once, in
// [GlobalSetup], and only queryRange's own lookup cost is measured. CreateByLinearScan
// checks every stored range for full containment, O(n) per query. CreateBy
// IntervalSetBinarySearch instead composes this repo's own IntervalSet<int> with
// BinarySearch.UpperBound over a Starts view to find the one candidate range that
// could contain the query, O(log n) per query.
[MemoryDiagnoser]
public class RangeModuleBenchmarks
{
    private const int RangeWidth = 2;
    private const int Stride = 4;
    private const int RandomSeed = 17;

    [Params(200, 5_000)]
    public int Length;

    private RangeModuleSolution.IRangeModule _linearScan = null!;
    private RangeModuleSolution.IRangeModule _intervalSetBinarySearch = null!;
    private (int Left, int Right)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        _linearScan = RangeModuleSolution.CreateByLinearScan();
        _intervalSetBinarySearch = RangeModuleSolution.CreateByIntervalSetBinarySearch();

        for (var i = 0; i < Length; i++)
        {
            var start = i * Stride;
            var end = start + RangeWidth;
            _linearScan.AddRange(start, end);
            _intervalSetBinarySearch.AddRange(start, end);
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
            if (_linearScan.QueryRange(left, right))
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
            if (_intervalSetBinarySearch.QueryRange(left, right))
            {
                trueCount++;
            }
        }

        return trueCount;
    }
}
