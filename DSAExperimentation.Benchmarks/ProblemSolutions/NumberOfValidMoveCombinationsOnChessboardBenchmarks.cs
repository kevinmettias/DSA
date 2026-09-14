using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfValidMoveCombinationsOnChessboard;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfValidMoveCombinationsOnChessboardSolution's,
// the same methods NumberOfValidMoveCombinationsOnChessboardTests proves correct.
// [GlobalSetup] slices the fixed four-corner board down to the measured piece
// count, which is already LeetCode's own (pieces, positions) input shape, so no
// separate hoisted overload is needed.
[MemoryDiagnoser]
public class NumberOfValidMoveCombinationsOnChessboardBenchmarks
{
    private static readonly string[] AllPieceTypes = ["rook", "queen", "bishop", "rook"];
    private static readonly int[][] AllPositions = [[1, 1], [8, 8], [1, 8], [8, 1]];

    [Params(2, 4)]
    public int PieceCount;

    private string[] _pieceTypes = null!;
    private int[][] _positions = null!;

    [GlobalSetup]
    public void Setup()
    {
        _pieceTypes = AllPieceTypes[..PieceCount];
        _positions = AllPositions[..PieceCount];
    }

    [Benchmark(Baseline = true)]
    public int CartesianProductThenValidate() =>
        NumberOfValidMoveCombinationsOnChessboardSolution.CountCombinationsByCartesianProduct(
            _pieceTypes, _positions);

    [Benchmark]
    public int PrunedBacktracking() =>
        NumberOfValidMoveCombinationsOnChessboardSolution.CountCombinationsByPrunedBacktracking(
            _pieceTypes, _positions);
}
