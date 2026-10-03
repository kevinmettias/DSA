using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumNumberOfGroupsGettingFreshDonuts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfGroupsGettingFreshDonutsSolution's, the
// same strategies MaximumNumberOfGroupsGettingFreshDonutsSolutionTests proves correct.
//
// Sizes are per arm. The baseline scores all n! orderings, so it stops at 9 groups;
// the memoized (residue, remaining remainder counts) search is there to collapse
// exactly that growth - with a fixed batch size its states grow only polynomially in
// the group count - and it runs on to LC 1815's own bound of 30. The two are compared
// at the counts both run.
public class MaximumNumberOfGroupsGettingFreshDonutsBenchmarks
{
    private const int BatchSize = 5;
    private const int MaxGroupSizeExclusive = 50;
    private const int GroupSeed = 1;

    private Dictionary<int, int[]> _groupsByCount = [];

    public static IEnumerable<int> AllPermutationsSizes => [6, 9];

    public static IEnumerable<int> MemoizedSearchSizes => [.. AllPermutationsSizes, 20, 30];

    // Every group count any arm runs is drawn here, outside the timed region, each from its
    // own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _groupsByCount = MemoizedSearchSizes.ToDictionary(
            count => count,
            count => SeededDraws.Values(count, 1, MaxGroupSizeExclusive, new Random(GroupSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(AllPermutationsSizes))]
    public int AllPermutations(int groupCount) =>
        MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByAllPermutations(
            BatchSize, _groupsByCount[groupCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSearchSizes))]
    public int MemoizedSearch(int groupCount) =>
        MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByMemoizedRecurrence(
            BatchSize, _groupsByCount[groupCount]);
}
