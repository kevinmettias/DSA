using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NQueensII;

public sealed partial class NQueensIITests
{
    [Theory]
    [InlineData(4, 2)]
    [InlineData(1, 1)]
    public void TotalNQueens_LeetCodeExamples_ReturnsSolutionCount(int n, int expected) => Assert.Equal(expected, Count(n));
    private static int Count(int n) { var count = 0; var state = new State(n); Backtrack.Search<State, int>(state, s => s.Row == n, s => s.Row == n ? [] : Enumerable.Range(0, n).Where(s.CanPlace), (s, col) => s.Place(col), (s, col) => s.Remove(col), _ => count++); return count; }
    private sealed class State(int n) { private readonly bool[] _cols = new bool[n]; private readonly bool[] _diag = new bool[(2 * n) - 1]; private readonly bool[] _anti = new bool[(2 * n) - 1]; public int Row { get; private set; } public bool CanPlace(int col) => !_cols[col] && !_diag[Row - col + n - 1] && !_anti[Row + col]; public void Place(int col) { _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = true; Row++; } public void Remove(int col) { Row--; _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = false; } }
}
