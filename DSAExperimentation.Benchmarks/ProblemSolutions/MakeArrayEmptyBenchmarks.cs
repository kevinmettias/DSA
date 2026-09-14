using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MakeArrayEmpty;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MakeArrayEmptySolution's, the same methods
// MakeArrayEmptyTests proves correct - the O(n) present-count scan against the
// FenwickTree<int, SumOperation<int>> range query at O(log n) per step, the same
// contrast CountGoodTripletsInAnArrayBenchmarks draws for LC 2179. [GlobalSetup]
// shuffles the distinct values, so only the sweep is measured.
[MemoryDiagnoser]
public class MakeArrayEmptyBenchmarks
{
    private const int Seed = 2659; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nums = ShuffledDistinctValues(new Random(Seed), Length);
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => MakeArrayEmptySolution.CountOperationsByLinearScan(_nums);

    [Benchmark]
    public long FenwickTreeSweep() => MakeArrayEmptySolution.CountOperationsByFenwickTree(_nums);

    private static int[] ShuffledDistinctValues(Random random, int length)
    {
        var values = Enumerable.Range(0, length).ToArray();

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }
}
