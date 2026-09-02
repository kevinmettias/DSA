using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ManhattanDistancesOfAllArrangementsOfPieces;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ManhattanDistancesOfAllArrangementsOfPiecesSolution's,
// the same methods ManhattanDistancesOfAllArrangementsOfPiecesTests proves correct.
// The grid is fixed and small - SumByBruteForceArrangements enumerates C(m*n, k)
// arrangements outright, so K is the only axis that can move without making the
// baseline arm impractical. SumByPairwiseDistanceFormula would happily take a
// grid with m*n in the tens of thousands instead.
[MemoryDiagnoser]
public class ManhattanDistancesOfAllArrangementsOfPiecesBenchmarks
{
    private const int Rows = 4;
    private const int Columns = 4;

    [Params(2, 4, 6)]
    public int K;

    [Benchmark(Baseline = true)]
    public long BruteForceArrangements() =>
        ManhattanDistancesOfAllArrangementsOfPiecesSolution.SumByBruteForceArrangements(Rows, Columns, K);

    [Benchmark]
    public long PairwiseDistanceFormula() =>
        ManhattanDistancesOfAllArrangementsOfPiecesSolution.SumByPairwiseDistanceFormula(Rows, Columns, K);
}
