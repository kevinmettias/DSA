using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ThreeSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ThreeSumSolution's, the same methods
// ThreeSumTests proves correct. Cubic duplicate-filtered brute force vs.
// MergeSort plus the sorted two-pointer sweep.
[MemoryDiagnoser]
public class ThreeSumBenchmarks
{
    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 15;

    private int[] _values = null!;

    [Params(80, 300)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-Length, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<(int First, int Second, int Third)> BruteForce() =>
        ThreeSumSolution.FindTripletsByBruteForce(_values);

    [Benchmark]
    public List<(int First, int Second, int Third)> MergeSortTwoPointers() =>
        ThreeSumSolution.FindTripletsByMergeSortTwoPointers(_values);
}
