using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchInsertPosition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SearchInsertPositionBenchmarks
{
    // Values are spaced by this step (even numbers only); the search target is derived
    // from the same step so it always falls strictly between two elements.
    private const int ElementStep = 2;

    private int[] _values = [];
    private int _target;
    [Params(200, 5_000)] public int Length { get; set; }
    [GlobalSetup] public void Setup()
    {
        _values = Enumerable.Range(0, Length).Select(i => i * ElementStep).ToArray();
        _target = (Length * ElementStep) - 1;
    }
    [Benchmark(Baseline = true)] public int LinearScan() => SearchInsertPositionSolution.SearchInsertByLinearScan(_values, _target);
    [Benchmark] public int BinarySearchLowerBound() => SearchInsertPositionSolution.SearchInsertByBinarySearchLowerBound(_values, _target);
}
