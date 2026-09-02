using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MostFrequentPrime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MostFrequentPrimeSolution's, the same methods
// MostFrequentPrimeTests proves correct. Grid construction is charged to
// [GlobalSetup]; the sieve arm still builds its own sieve inside the
// measured call, since that precompute is sized from the grid itself and is
// the composition being measured, not input construction.
[MemoryDiagnoser]
public class MostFrequentPrimeBenchmarks
{
    private const int GridSeed = 3044;

    [Params(2, 4, 6)]
    public int GridSize;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = DigitGridWorkloads.BuildGrid(GridSize, seed: GridSeed);

    [Benchmark(Baseline = true)]
    public int TrialDivision() => MostFrequentPrimeSolution.MostFrequentPrimeByTrialDivision(_grid);

    [Benchmark]
    public int Sieve() => MostFrequentPrimeSolution.MostFrequentPrimeBySieve(_grid);
}
