using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheScoreDifferenceInAGameSolution's, the same
// methods FindTheScoreDifferenceInAGameSolutionTests proves correct. Neither arm gets a
// prepared-input overload: the "expensive" input each would otherwise share (the
// marked array, the heap) is exactly what the simulation consumes turn by turn, so
// there is nothing left to charge to [GlobalSetup] beyond the raw array itself.
// LC 3847 caps the games at 1000 and the points at 1..1000, so the larger Length is
// that cap and every score is drawn from that range.
public class FindTheScoreDifferenceInAGameBenchmarks
{
    private const int RandomSeed = 3847;
    // One past LC 3847's largest score, 1000.
    private const int ScoreUpperBoundExclusive = 1_001;

    private int[] _nums = [];

    [Params(500, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, ScoreUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByLinearScan(_nums);

    [Benchmark]
    public int MinHeap() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByMinHeap(_nums);
}
