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
    // arm CanTraceWordByBacktrack has to justify itself against.
    public static bool CanTraceWordByBruteForceDfs(char[][] board, string word)
    {
        var used = new bool[board.Length, board[0].Length];
        var walk = new WordWalk(board, word, used);

        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (CanTraceRemainingFrom((row, col), 0, walk))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // This repo's own choose/explore/unchoose engine: State tracks the visited grid
    // and a coordinate trail so Unchoose can restore exactly what Choose changed,
    // and OnSolution returning true stops the whole search the moment one path
    // spells out the word.
    public static bool CanTraceWordByBacktrack(char[][] board, string word)
    {
        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (CanTraceWordFrom(board, word, row, col))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool CanTraceWordFrom(char[][] board, string word, int row, int col)
    {
        var state = new State(board, word, row, col);
        return Backtrack.TrySearch(state, new BacktrackingSteps<State, (int Row, int Col)>(
            IsSolution: s => s.Index == s.Word.Length,
            Candidates: s => s.Index == s.Word.Length ? Array.Empty<(int Row, int Col)>() : s.Candidates(),
            Choose: (s, p) => s.Choose(p),
            Unchoose: (s, p) => s.Unchoose(p),
            OnSolution: _ => true));
    }

    // The board, the word and the used-path grid are the same three the whole search
    // runs against, so they travel as one argument; what recurses is only where the
    // path stands and how far along the word it has reached.
    private static bool CanTraceRemainingFrom((int Row, int Col) cell, int index, WordWalk walk)
    {
        if (index == walk.Word.Length)
        {
            return true;
        }

        if (!CanExtendWordWith(walk.Board, walk.Used, cell, walk.Word[index]))
        {
            return false;
        }

        var (row, col) = cell;
        walk.Used[row, col] = true;

        var found = CanTraceRemainingFrom((row + 1, col), index + 1, walk)
            || CanTraceRemainingFrom((row - 1, col), index + 1, walk)
            || CanTraceRemainingFrom((row, col + 1), index + 1, walk)
            || CanTraceRemainingFrom((row, col - 1), index + 1, walk);

        walk.Used[row, col] = false;
        return found;
    }

    // The cell can carry the word's next letter: it is on the board, not already
    // used by this path, and holds the letter the word expects there.
    private static bool CanExtendWordWith(
        char[][] board, bool[,] used, (int Row, int Col) cell, char expected)
        => cell.Row >= 0 && cell.Row < board.Length
            && cell.Col >= 0 && cell.Col < board[0].Length
            && !used[cell.Row, cell.Col]
            && board[cell.Row][cell.Col] == expected;

    private sealed class State(char[][] board, string word, int startRow, int startCol)
    {
        private readonly bool[,] _used = new bool[board.Length, board[0].Length];
        private readonly Stack<(int Row, int Col)> _parents = new();
        private int Row { get; set; } = startRow;
        private int Col { get; set; } = startCol;
        public string Word => word;
        public int Index { get; private set; }

        public IEnumerable<(int Row, int Col)> Candidates()
        {
            foreach (var p in Neighbors())
            {
                if (CanExtendWordWith(board, _used, p, word[Index]))
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

        public void Choose((int Row, int Col) cell)
        {
            _parents.Push((Row, Col));
            Row = cell.Row;
            Col = cell.Col;
            _used[Row, Col] = true;
            Index++;
        }

        public void Unchoose((int Row, int Col) cell)
        {
            Index--;
            _used[cell.Row, cell.Col] = false;
            (Row, Col) = _parents.Pop();
        }
    }

    // What the hand-rolled walk reads and marks and never replaces: the board it spells
    // on, the word it spells, and the grid of cells the path has already used. Only the
    // cell it stands on and how far along the word it has reached differ per call.
    private readonly record struct WordWalk(char[][] Board, string Word, bool[,] Used);
}
