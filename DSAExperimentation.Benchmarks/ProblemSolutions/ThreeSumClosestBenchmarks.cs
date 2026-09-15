using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ThreeSumClosest;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 3Sum Closest (LC 16): harness only. Both arms are ThreeSumClosestSolution's, the
// same methods ThreeSumClosestTests proves correct - cubic exhaustive scan vs.
// MergeSort plus a linear two-pointer sweep per fixed first element.
[MemoryDiagnoser]
public class ThreeSumClosestBenchmarks
{
    private const int Target = 37;

    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 16;

    private int[] _values = [];

    [Params(80, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-Length, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => ThreeSumClosestSolution.ClosestSumByBruteForce(_values, Target);

    [Benchmark]
    public int MergeSortTwoPointers() => ThreeSumClosestSolution.ClosestSumByMergeSortTwoPointers(_values, Target);
}
