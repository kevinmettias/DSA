using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumXOROfTwoNumbersInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumXOROfTwoNumbersInAnArraySolution's, the same
// methods MaximumXOROfTwoNumbersInAnArrayTests proves correct - the textbook O(n^2)
// pairwise scan vs. this repo's own BitTrie greedy walk, O(n).
[MemoryDiagnoser]
public class MaximumXOROfTwoNumbersInAnArrayBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, int.MaxValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() => MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByPairwiseScan(_values);

    [Benchmark]
    public int BitTrieGreedy() => MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByBitTrie(_values);
}
