using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountWaysToChooseCoprimeIntegersFromRows;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountWaysToChooseCoprimeIntegersFromRowsSolution's,
// the same methods CountWaysToChooseCoprimeIntegersFromRowsTests proves correct.
//
// Size stays a square matrix well below LC's own 150x150 bound - the brute-force
// DFS enumerates size^size leaf combinations and would not finish otherwise; the
// GCD-counting DP strategy stays polynomial in size regardless.
[MemoryDiagnoser]
public class CountWaysToChooseCoprimeIntegersFromRowsBenchmarks
{
    private const int RandomSeed = 3725; // LC problem number
    private const int MaxValueInclusive = 150;

    private int[][] _mat = [];

    [Params(5, 7)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _mat = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _mat[row] = Enumerable.Range(0, Size).Select(_ => random.Next(1, MaxValueInclusive + 1)).ToArray();
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByBruteForce(_mat);

    [Benchmark]
    public long GcdCountingDp() => CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByGcdCountingDp(_mat);
}
