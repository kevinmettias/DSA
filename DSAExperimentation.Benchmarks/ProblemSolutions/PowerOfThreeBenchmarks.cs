using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.PowerOfThree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerOfThreeSolution's. The binary search arm takes
// the hoisted ArraySequence<int> overload so wrapping the (already-precomputed)
// powers-of-three table is not charged to the measured search.
[MemoryDiagnoser]
public class PowerOfThreeBenchmarks
{
    private ArraySequence<int> _powersOfThree;

    [Params(1162261467, 1162261466)] // 3^19 (a true power of three) vs. one less (not)
    public int Value { get; set; }

    [GlobalSetup]
    public void Setup() => _powersOfThree = new ArraySequence<int>(PowersOfThreeTable.Powers);

    [Benchmark(Baseline = true)]
    public bool IsPowerOfThreeByDivisionLoop() => PowerOfThreeSolution.IsPowerOfThreeByDivisionLoop(Value);

    [Benchmark]
    public bool IsPowerOfThreeByBinarySearch() => PowerOfThreeSolution.IsPowerOfThreeByBinarySearch(Value, _powersOfThree);
}
