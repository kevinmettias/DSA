using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfGroupsGettingFreshDonuts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfGroupsGettingFreshDonutsSolution's, the
// same strategies MaximumNumberOfGroupsGettingFreshDonutsTests proves correct. The
// group count stays small because the baseline scores all n! orderings, which is
// exactly the growth the memoized (residue, remaining remainder counts) search is
// there to collapse.
[MemoryDiagnoser]
public class MaximumNumberOfGroupsGettingFreshDonutsBenchmarks
{
    private const int BatchSize = 5;
    private const int MaxGroupSizeExclusive = 50;
    private const int GroupSeed = 1;

    private int[] _groups = [];

    [Params(6, 9)]
    public int GroupCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(GroupSeed);
        _groups = Enumerable.Range(0, GroupCount).Select(_ => random.Next(1, MaxGroupSizeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int AllPermutations() =>
        MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByAllPermutations(BatchSize, _groups);

    [Benchmark]
    public int MemoizedSearch() =>
        MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByMemoizedRecurrence(BatchSize, _groups);
}
