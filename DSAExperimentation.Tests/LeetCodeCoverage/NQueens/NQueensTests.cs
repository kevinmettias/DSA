using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NQueens;

public sealed partial class NQueensTests
{
    [Fact]
    public void SolveNQueens_FourQueens_ReturnsTwoSolutions()
    {
        var solutions = Solve(4);
        Assert.Equal(2, solutions.Count);
        Assert.Contains(solutions, board => board.SequenceEqual([".Q..", "...Q", "Q...", "..Q."]));
    }

    private static List<List<string>> Solve(int n)
    {
        var results = new List<List<string>>();
        var state = new State(n);
        Backtrack.Search<State, int>(state, s => s.Row == n, s => s.Row == n ? [] : Enumerable.Range(0, n).Where(s.CanPlace), (s, col) => s.Place(col), (s, col) => s.Remove(col), s => results.Add(s.Board()));
        return results;
    }

    private sealed class State(int n)
    {
        private readonly bool[] _cols = new bool[n];
        private readonly bool[] _diag = new bool[(2 * n) - 1];
        private readonly bool[] _anti = new bool[(2 * n) - 1];
        private readonly int[] _placed = new int[n];
        public int Row { get; private set; }
        public bool CanPlace(int col) => !_cols[col] && !_diag[Row - col + n - 1] && !_anti[Row + col];
        public void Place(int col) { _placed[Row] = col; _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = true; Row++; }
        public void Remove(int col) { Row--; _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = false; }
        public List<string> Board() => Enumerable.Range(0, n).Select(row => new string(Enumerable.Range(0, n).Select(col => _placed[row] == col ? 'Q' : '.').ToArray())).ToList();
    }
}
