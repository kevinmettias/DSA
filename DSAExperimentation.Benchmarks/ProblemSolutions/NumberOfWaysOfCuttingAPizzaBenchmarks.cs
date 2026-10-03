using DSAExperimentation.LeetCode.NumberOfWaysOfCuttingAPizza;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysOfCuttingAPizzaSolution's, the same
// methods NumberOfWaysOfCuttingAPizzaSolutionTests proves correct. Both are handed the
// prepared AppleGrid their hoisted overload takes, so building the suffix-sum table
// is charged to [GlobalSetup] rather than to the cut counting being measured. The
// pizza is all apples, so nothing short-circuits the naive baseline's full
// branching early.
//
// Sizes are per arm. The naive baseline stops at an 8 x 8 pizza; the memoized arm's
// (row, col, cuts) states run on to LC 1444's own bound of 50 x 50. The two are
// compared at the sizes both run.
public class NumberOfWaysOfCuttingAPizzaBenchmarks
{
    // Four cuts, the workload the pre-migration benchmark measured, stated here as
    // the piece count LeetCode's own signature takes.
    private const int Pieces = 5;

    private Dictionary<int, AppleGrid> _applesBySize = [];

    public static IEnumerable<int> BaselineSizes => [6, 8];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 20, 50];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _applesBySize = MemoizedSizes.ToDictionary(size => size, BuildApples);

    private static AppleGrid BuildApples(int size)
    {
        var pizza = new string[size];
        Array.Fill(pizza, new string('A', size));

        return new AppleGrid(pizza);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int UnmemoizedRecursion(int size) =>
        NumberOfWaysOfCuttingAPizzaSolution.CountWaysByUnmemoizedRecursion(_applesBySize[size], Pieces);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int size) =>
        NumberOfWaysOfCuttingAPizzaSolution.CountWaysByMemoizedRecursion(_applesBySize[size], Pieces);
}
