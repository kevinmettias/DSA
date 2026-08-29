using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordSearch;

public sealed partial class WordSearchTests
{
    [Theory]
    [InlineData("ABCCED", true)]
    [InlineData("SEE", true)]
    [InlineData("ABCB", false)]
    public void Exist_LeetCodeExamples_ReturnsWhetherWordCanBeTraced(string word, bool expected)
    {
        char[][] board = [['A','B','C','E'], ['S','F','C','S'], ['A','D','E','E']];
        Assert.Equal(expected, Exist(board, word));
    }

    private static bool Exist(char[][] board, string word)
    {
        for (var row = 0; row < board.Length; row++)
        for (var col = 0; col < board[0].Length; col++)
        {
            if (SearchFrom(board, word, row, col)) return true;
        }
        return false;
    }

    private static bool SearchFrom(char[][] board, string word, int row, int col)
    {
        var state = new State(board, word, row, col);
        return Backtrack.TrySearch(state, new BacktrackingSteps<State, (int Row, int Col)>(
            IsSolution: s => s.Index == s.Word.Length,
            Candidates: s => s.Index == s.Word.Length ? [] : s.Candidates(),
            Choose: (s, p) => s.Choose(p),
            Unchoose: (s, p) => s.Unchoose(p),
            OnSolution: _ => true));
    }

    private sealed class State(char[][] board, string word, int startRow, int startCol)
    {
        private readonly bool[,] _used = new bool[board.Length, board[0].Length];
        private readonly Stack<(int Row, int Col)> _parents = new();
        public string Word => word;
        public int Index { get; private set; }
        private int Row { get; set; } = startRow;
        private int Col { get; set; } = startCol;

        public IEnumerable<(int Row, int Col)> Candidates()
        {
            foreach (var p in Neighbors())
            {
                if (p.Row >= 0 && p.Row < board.Length && p.Col >= 0 && p.Col < board[0].Length
                    && !_used[p.Row, p.Col] && board[p.Row][p.Col] == word[Index])
                {
                    yield return p;
                }
            }
        }

        private IEnumerable<(int Row, int Col)> Neighbors()
        {
            if (Index == 0)
            {
                yield return (Row, Col);
                yield break;
            }

            yield return (Row + 1, Col);
            yield return (Row - 1, Col);
            yield return (Row, Col + 1);
            yield return (Row, Col - 1);
        }

        public void Choose((int Row, int Col) p)
        {
            _parents.Push((Row, Col));
            Row = p.Row;
            Col = p.Col;
            _used[Row, Col] = true;
            Index++;
        }

        public void Unchoose((int Row, int Col) p)
        {
            Index--;
            _used[p.Row, p.Col] = false;
            (Row, Col) = _parents.Pop();
        }
    }
}
