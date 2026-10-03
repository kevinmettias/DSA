using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ValidateStackSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidateStackSequencesSolution's, the same methods
// ValidateStackSequencesSolutionTests proves correct - exhaustive push/pop-timing
// backtracking (O(2^n) worst case, undoing a branch on failure) against the single
// O(n) greedy sweep through this repo's own Stack<int>. The pushed/popped pair is a
// genuinely valid one, built once in [GlobalSetup], so the backtracking arm has to
// search rather than fail fast.
//
// Sizes are per arm. The backtracking search stops at 16; the greedy sweep runs on to
// LC 946's own bound of 1,000. The two are compared at the sizes both run.
public class ValidateStackSequencesBenchmarks
{
    // LC problem number, reused as the deterministic interleaving seed.
    private const int RandomSeed = 946;

    private Dictionary<int, (int[] Pushed, int[] Popped)> _sequencesBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 16];

    public static IEnumerable<int> GreedySweepSizes => [.. BaselineSizes, 100, 1_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _sequencesBySize = GreedySweepSizes.ToDictionary(length => length, BuildSequences);

    private static (int[] Pushed, int[] Popped) BuildSequences(int length)
    {
        var pushed = StackSequenceWorkloads.BuildPushed(length);

        return (pushed, StackSequenceWorkloads.BuildValidPopOrder(pushed, RandomSeed));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool IsValidByBacktrackingSearch(int length)
    {
        var (pushed, popped) = _sequencesBySize[length];

        return ValidateStackSequencesSolution.IsValidByBacktrackingSearch(pushed, popped);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GreedySweepSizes))]
    public bool IsValidByGreedyStackSweep(int length)
    {
        var (pushed, popped) = _sequencesBySize[length];

        return ValidateStackSequencesSolution.IsValidByGreedyStackSweep(pushed, popped);
    }
}
