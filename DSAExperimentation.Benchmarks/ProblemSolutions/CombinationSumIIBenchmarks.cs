using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CombinationSumII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIISolution's, now returning the actual
// combinations the test proves correct instead of merely counting them. Candidates
// are drawn from a seeded Random across LC 40's whole [1, 50] and searched against
// its largest target, 30, so the larger size repeats most values the search can use
// and the duplicate skip the "II" variant exists for is exercised throughout.
// CandidateCount stops at LC 40's 100-candidate cap.
public class CombinationSumIIBenchmarks
{
    private const int RandomSeed = 40; // LC problem number
    private const int MaxCandidate = 50;
    private const int Target = 30;

    private int[] _candidates = [];

    [Params(25, 100)]
    public int CandidateCount { get; set; }

    [GlobalSetup]
    public void Setup() => _candidates = SeededDraws.Values(CandidateCount, 1, MaxCandidate + 1, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public List<List<int>> SortAndBacktrackSpecialized() =>
        CombinationSumIISolution.FindCombinationsBySpecializedRecursion(_candidates, Target);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        CombinationSumIISolution.FindCombinationsByBacktracking(_candidates, Target);
}
