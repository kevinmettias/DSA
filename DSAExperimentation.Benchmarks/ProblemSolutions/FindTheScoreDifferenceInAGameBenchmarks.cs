using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheScoreDifferenceInAGameSolution's, the same
// methods FindTheScoreDifferenceInAGameSolutionTests proves correct - one O(n) pass
// each, the role simulation branching on who is active and the parity pass folding
// every game into one signed sum. Points are drawn from LC 3847's 1..1000, so about
// half are odd and the active player changes unpredictably; LC caps the games at
// 1000, so the larger Length is that cap.
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
    public int RoleSimulation() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByRoleSimulation(_nums);

    [Benchmark]
    public int SwapParity() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceBySwapParity(_nums);
}
