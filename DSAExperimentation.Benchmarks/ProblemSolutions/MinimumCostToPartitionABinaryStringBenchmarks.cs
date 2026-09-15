using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.LeetCode.MinimumCostToPartitionABinaryString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToPartitionABinaryStringSolution's,
// the same methods MinimumCostToPartitionABinaryStringTests proves correct.
// The Fenwick arm is handed the prepared FenwickTree its hoisted overload
// takes, so building it is charged to [GlobalSetup] rather than to the
// recursion being measured - the same split OpenTheLockBenchmarks uses for
// its LockGraph arm. Length is a power of two so the split-in-half recursion
// bottoms out at single characters instead of stopping early on an odd
// length, giving both arms their deepest possible recursion tree.
[MemoryDiagnoser]
public class MinimumCostToPartitionABinaryStringBenchmarks
{
    private const int Seed = 3864;
    private const int EncCost = 7;
    private const int FlatCost = 11;

    private string _s = "";

    private FenwickTree<int, SumOperation<int>> _sensitiveCounts = null!;
    [Params(1024, 8192)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var characters = new char[Length];
        var sensitive = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            var isSensitive = random.Next(2) == 1;
            characters[i] = isSensitive ? '1' : '0';
            sensitive[i] = isSensitive ? 1 : 0;
        }

        _s = new string(characters);
        _sensitiveCounts = new FenwickTree<int, SumOperation<int>>(sensitive);
    }

    [Benchmark(Baseline = true)]
    public long LinearScanRecursion() =>
        MinimumCostToPartitionABinaryStringSolution.MinCostByLinearScanRecursion(_s, EncCost, FlatCost);

    [Benchmark]
    public long FenwickRangeSum() =>
        MinimumCostToPartitionABinaryStringSolution.MinCostByFenwickRangeSum(_sensitiveCounts, Length, EncCost, FlatCost);
}
