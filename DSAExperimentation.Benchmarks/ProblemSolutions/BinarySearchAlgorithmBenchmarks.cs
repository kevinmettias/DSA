using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BinarySearchAlgorithm;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinarySearchAlgorithmSolution's, the same methods
// BinarySearchAlgorithmTests proves correct.
[MemoryDiagnoser]
public class BinarySearchAlgorithmBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        _values = BinarySearchAlgorithmWorkloads.BuildSortedValues(Length);
        _target = BinarySearchAlgorithmWorkloads.FarthestTarget(Length);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => BinarySearchAlgorithmSolution.FindIndexByLinearScan(_values, _target);

    [Benchmark]
    public int BinarySearchFind() => BinarySearchAlgorithmSolution.FindIndexByBinarySearch(_values, _target);
}
