using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class ValidSudokuBenchmarks
{
    private char[][] _board = null!;
    [GlobalSetup] public void Setup() => _board = [['5','3','.','.','7','.','.','.','.'], ['6','.','.','1','9','5','.','.','.'], ['.','9','8','.','.','.','.','6','.'], ['8','.','.','.','6','.','.','.','3'], ['4','.','.','8','.','3','.','.','1'], ['7','.','.','.','2','.','.','.','6'], ['.','6','.','.','.','.','2','8','.'], ['.','.','.','4','1','9','.','.','5'], ['.','.','.','.','8','.','.','7','9']];
    [Benchmark(Baseline = true)] public bool BooleanArrays() { var rows = new bool[9, 10]; var cols = new bool[9, 10]; var boxes = new bool[9, 10]; for (var r = 0; r < 9; r++) for (var c = 0; c < 9; c++) { var ch = _board[r][c]; if (ch == '.') continue; var d = ch - '0'; var b = (r / 3 * 3) + (c / 3); if (rows[r, d] || cols[c, d] || boxes[b, d]) return false; rows[r, d] = cols[c, d] = boxes[b, d] = true; } return true; }
    [Benchmark] public bool SetKeys() { var seen = new Set<string>(); for (var r = 0; r < 9; r++) for (var c = 0; c < 9; c++) { var d = _board[r][c]; if (d == '.') continue; if (!seen.TryAdd($"r{r}:{d}") || !seen.TryAdd($"c{c}:{d}") || !seen.TryAdd($"b{r / 3},{c / 3}:{d}")) return false; } return true; }
}
