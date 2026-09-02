using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheNumberOfInfectionSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfInfectionSequencesSolution's, the
// same methods CountTheNumberOfInfectionSequencesTests proves agree.
//
// sick = [0, n-1] leaves a single interior run spanning the whole middle of the
// line - the run shape where the brute-force simulation branches hardest (two
// live candidates, one at each end of the run, until the very last move), so
// QueueLength has to stay small enough for that 2^(n-3)-leaf enumeration to
// finish. The closed-form strategy is O(n) regardless and would scale far past
// what is measured here.
[MemoryDiagnoser]
public class CountTheNumberOfInfectionSequencesBenchmarks
{
    [Params(12, 18)]
    public int QueueLength;

    private int[] _sick = null!;

    [GlobalSetup]
    public void Setup() => _sick = [0, QueueLength - 1];

    [Benchmark(Baseline = true)]
    public long BruteForceSimulation() =>
        CountTheNumberOfInfectionSequencesSolution.CountSequencesByBruteForceSimulation(QueueLength, _sick);

    [Benchmark]
    public long GapCombinatorics() =>
        CountTheNumberOfInfectionSequencesSolution.CountSequencesByGapCombinatorics(QueueLength, _sick);
}
