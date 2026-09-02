using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.WordSearch;

// LeetCode 79. Word Search: does a path of adjacent cells, starting anywhere on the
// board and never reusing a cell, spell out the target word.
//
// The two strategies differ only in whether the choose/explore/unchoose walk goes
// through this repo's own Backtrack primitive (with a State object carrying the
// visited grid and the coordinate trail) or is written out by hand as a plain
// recursive function closing over a used[,] array - the textbook shape the
// composed solution is measured against.
internal static class WordSearchSolution
{
    // The textbook answer: a hand-rolled recursive DFS over a BCL bool[,] visited
    // grid, deliberately written without this repo's Backtracking primitive - the
    // arm ExistByBacktrack has to justify itself against.
    public static bool ExistByBruteForceDfs(char[][] board, string word)
    {
        var used = new bool[board.Length, board[0].Length];

        for (var row = 0; row < board.Length; row++)
        for (var col = 0; col < board[0].Length; col++)
        {
            if (Search(board, word, used, row, col, 0))
            {
                return true;
            }
        }

        return false;
    }

    private static bool Search(char[][] board, string word, bool[,] used, int row, int col, int index)
    {
        if (index == word.Length)
        {
            return true;
        }

        if (row < 0 || row >= board.Length || col < 0 || col >= board[0].Length
            || used[row, col] || board[row][col] != word[index])
        {
            return false;
        }

        used[row, col] = true;

        var found = Search(board, word, used, row + 1, col, index + 1)
            || Search(board, word, used, row - 1, col, index + 1)
            || Search(board, word, used, row, col + 1, index + 1)
            || Search(board, word, used, row, col - 1, index + 1);

        used[row, col] = false;
        return found;
    }

    // This repo's own choose/explore/unchoose engine: State tracks the visited grid
    // and a coordinate trail so Unchoose can restore exactly what Choose changed,
    // and OnSolution returning true stops the whole search the moment one path
    // spells out the word.
    public static bool ExistByBacktrack(char[][] board, string word)
    {
        for (var row = 0; row < board.Length; row++)
        for (var col = 0; col < board[0].Length; col++)
        {
            if (SearchFrom(board, word, row, col))
            {
                return true;
            }
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
