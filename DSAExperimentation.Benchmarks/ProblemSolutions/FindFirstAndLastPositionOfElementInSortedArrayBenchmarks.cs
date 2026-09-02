using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindFirstAndLastPositionOfElementInSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class FindFirstAndLastPositionOfElementInSortedArrayBenchmarks
{
    private const int ValueDuplicationFactor = 4;
    private const int TargetDivisor = 8;

    private int[] _values = null!;
    private int _target;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() { _values = Enumerable.Range(0, Length).Select(i => i / ValueDuplicationFactor).ToArray(); _target = Length / TargetDivisor; }
    [Benchmark(Baseline = true)] public int[] LinearScan() => FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByLinearScan(_values, _target);
    [Benchmark] public int[] BinarySearchBounds() => FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByBinarySearchBounds(_values, _target);
}
