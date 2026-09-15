using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MatchsticksToSquare;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MatchsticksToSquareSolution's. _matchsticks is four
// interleaved copies of 1..SticksPerSide, so a perfect split always exists (each
// side re-assembles the copy it came from) but the shuffled ordering still forces
// a real search rather than an immediate match.
[MemoryDiagnoser]
public class MatchsticksToSquareBenchmarks
{
    private const int SquareSideCount = 4;

    private int[] _matchsticks = [];

    [Params(6, 8)]
    public int SticksPerSide { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var perSide = Enumerable.Range(1, SticksPerSide).ToArray();
        var all = new List<int>();

        for (var side = 0; side < SquareSideCount; side++)
        {
            all.AddRange(perSide);
        }

        var random = new Random(1);
        _matchsticks = all.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking() => MatchsticksToSquareSolution.CanMakeSquareByNaiveBacktracking(_matchsticks);

    [Benchmark]
    public bool GenericBacktrack() => MatchsticksToSquareSolution.CanMakeSquareByGenericBacktrack(_matchsticks);
}
