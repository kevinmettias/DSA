using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.WiggleSortII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WiggleSortIISolution's. Both strategies mutate their
// argument in place (LeetCode's own signature), so each arm first copies the shared
// workload into a buffer [GlobalSetup] allocated once and rearranges that. The copy is
// timed on purpose because the strategies mutate their input, and every arm pays the
// same cost.
public class WiggleSortIIBenchmarks
{
    private const int RandomSeed = 324; // LC problem number

    // Bounds random values to Length/2 so duplicates are common.
    private const int ValueRangeDivisor = 2;

    private int[] _values = [];
    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = WiggleSortIIWorkloads.BuildValuesWithDuplicates(Length, RandomSeed, ValueRangeDivisor);
        _nums = new int[Length];
    }

    [Benchmark(Baseline = true)]
    public int[] SelectionSortInterleave()
    {
        _values.CopyTo(_nums, 0);
        WiggleSortIISolution.WiggleSortBySelectionSort(_nums);
        return _nums;
    }

    [Benchmark]
    public int[] MergeSortInterleave()
    {
        _values.CopyTo(_nums, 0);
        WiggleSortIISolution.WiggleSortByMergeSort(_nums);
        return _nums;
    }
}
