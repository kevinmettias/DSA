using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumIncrementsForTargetMultiplesInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumIncrementsForTargetMultiplesInAnArraySolution's, the same methods MinimumIncrementsForTargetMultiplesInAnArrayTests
// proves correct. Each arm takes the hoisted TargetLcmTable so the (tiny, but still
// per-query) LCM precomputation is charged to [GlobalSetup] rather than the search
// being measured. NumsCount is kept in the low thousands, not LC's own 5*10^4 cap:
// the memoized arm recurses one stack frame per nums element, and this repo's
// Memoizer is explicitly real-recursion-only (see its own doc comment), so this
// stays within a safe recursion depth rather than chasing LC's own input ceiling.
[MemoryDiagnoser]
public class MinimumIncrementsForTargetMultiplesInAnArrayBenchmarks
{
    // LC problem number, reused as the deterministic nums seed.
    private const int NumsSeed = 3444;

    private static readonly int[] Target = [4, 6, 9, 10];

    private int[] _nums = [];

    private TargetLcmTable _lcmTable;
    [Params(100, 1_000)]
    public int NumsCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(NumsSeed);
        _nums = Enumerable.Range(0, NumsCount).Select(_ => random.Next(1, 10_000)).ToArray();
        _lcmTable = TargetLcmTable.Build(Target);
    }

    [Benchmark(Baseline = true)]
    public long BottomUpBitmaskDp() =>
        MinimumIncrementsForTargetMultiplesInAnArraySolution.MinIncrementsByBottomUpBitmaskDp(_nums, _lcmTable);

    [Benchmark]
    public long MemoizedBitmaskDp() =>
        MinimumIncrementsForTargetMultiplesInAnArraySolution.MinIncrementsByMemoizedBitmaskDp(_nums, _lcmTable);
}
