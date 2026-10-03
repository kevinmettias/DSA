using DSAExperimentation.LeetCode.CountTheNumberOfInfectionSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfInfectionSequencesSolution's, the
// same methods CountTheNumberOfInfectionSequencesSolutionTests proves agree.
//
// sick = [0, n-1] leaves a single interior run spanning the whole middle of the
// line - the run shape where the brute-force simulation branches hardest (two
// live candidates, one at each end of the run, until the very last move).
//
// Sizes are per arm. That 2^(n-3)-leaf enumeration stops at a queue of 18; the
// closed-form strategy is O(n) regardless and runs on to LC 2954's own bound of 10^5,
// and the two are compared at the lengths both run.
public class CountTheNumberOfInfectionSequencesBenchmarks
{
    private Dictionary<int, int[]> _sickByLength = [];

    public static IEnumerable<int> BruteForceSizes => [12, 18];

    public static IEnumerable<int> GapCombinatoricsSizes => [.. BruteForceSizes, 1_000, 100_000];

    // Every queue length any arm runs has its sick set built here, outside the timed
    // region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _sickByLength = GapCombinatoricsSizes.ToDictionary(length => length, length => new[] { 0, length - 1 });

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForceSimulation(int queueLength) =>
        CountTheNumberOfInfectionSequencesSolution.CountSequencesByBruteForceSimulation(
            queueLength, _sickByLength[queueLength]);

    [Benchmark]
    [ArgumentsSource(nameof(GapCombinatoricsSizes))]
    public long GapCombinatorics(int queueLength) =>
        CountTheNumberOfInfectionSequencesSolution.CountSequencesByGapCombinatorics(
            queueLength, _sickByLength[queueLength]);
}
