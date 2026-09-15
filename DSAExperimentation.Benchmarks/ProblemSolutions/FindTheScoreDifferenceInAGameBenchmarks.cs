using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheScoreDifferenceInAGameSolution's, the same
// methods FindTheScoreDifferenceInAGameTests proves correct. Neither arm gets a
// prepared-input overload: the "expensive" input each would otherwise share (the
// marked array, the heap) is exactly what the simulation consumes turn by turn, so
// there is nothing left to charge to [GlobalSetup] beyond the raw array itself.
[MemoryDiagnoser]
public class FindTheScoreDifferenceInAGameBenchmarks
{
    private const int RandomSeed = 3847;
    private const int MaxValueExclusive = 1_000_000;

    private int[] _nums = [];

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByLinearScan(_nums);

    [Benchmark]
    public int MinHeap() => FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByMinHeap(_nums);
}
