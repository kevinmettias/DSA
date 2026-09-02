using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.WiggleSortII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WiggleSortIISolution's. Both strategies mutate their
// argument in place (LeetCode's own signature), so each benchmark clones the shared
// workload before calling in - the clone is charged to the measured method exactly
// as before, only the sort itself moved to the solution tier.
[MemoryDiagnoser]
public class WiggleSortIIBenchmarks
{
    private const int RandomSeed = 324; // LC problem number
    private const int ValueRangeDivisor = 2; // bounds random values to Length/2 so duplicates are common

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() =>
        _values = WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, RandomSeed, ValueRangeDivisor);

    [Benchmark(Baseline = true)]
    public int[] SelectionSortInterleave()
    {
        var nums = (int[])_values.Clone();
        WiggleSortIISolution.WiggleSortBySelectionSort(nums);
        return nums;
    }

    [Benchmark]
    public int[] MergeSortInterleave()
    {
        var nums = (int[])_values.Clone();
        WiggleSortIISolution.WiggleSortByMergeSort(nums);
        return nums;
    }
}
