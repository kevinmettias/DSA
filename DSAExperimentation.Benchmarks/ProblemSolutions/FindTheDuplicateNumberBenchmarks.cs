using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheDuplicateNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheDuplicateNumberSolution's, the same methods
// FindTheDuplicateNumberTests proves correct - the canonical O(n^2) all-pairs brute
// force vs. treating nums[i] as a pointer from node i to node nums[i] and handing the
// resulting implicit linked list to this repo's own Floyd's-algorithm
// CycleDetection.FindCycleStart. _values is 1..Length with Length itself appended
// again, so the only matching pair is the very last one the brute force reaches,
// forcing its full O(n^2) scan.
[MemoryDiagnoser]
public class FindTheDuplicateNumberBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = new int[Length + 1];

        for (var i = 0; i < Length; i++)
        {
            _values[i] = i + 1;
        }

        _values[Length] = Length;
    }

    [Benchmark(Baseline = true)]
    public int NestedLoopBruteForce() => FindTheDuplicateNumberSolution.FindDuplicateByBruteForce(_values);

    [Benchmark]
    public int LinkedListCycleDetection() => FindTheDuplicateNumberSolution.FindDuplicateByCycleDetection(_values);
}
