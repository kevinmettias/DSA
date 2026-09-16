using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.WordSearchII;

// LeetCode 212. Word Search II: find every dictionary word that traces a path of
// horizontally/vertically adjacent board cells, never reusing a cell within one
// word. WordSearchTests (LC 79) already proves Backtrack.Search's choose/explore/
// unchoose walk finds one word on a board; finding every word from a whole
// dictionary at once composes exactly one more existing primitive -
// LowercaseTrie<TValue>, walked node-by-node alongside the board so a branch with
// no matching trie child is pruned before Candidates() ever yields it, instead of
// re-running a fresh single-word search per candidate word.
internal static class WordSearchIISolution
{
    // The textbook answer: WordSearchSolution's own per-word recursive DFS (LC 79),
    // re-run once per dictionary word from scratch over a BCL bool[,] visited grid -
    // O(words * cells * 4^L). Deliberately written without this repo's Backtracking
    // primitive or LowercaseTrie - the arm FindWordsByTrieBacktrack has to justify
    // itself against.
    public static HashSet<string> FindWordsByBruteForceDfs(char[][] board, string[] words)
    {
        var found = new HashSet<string>();

        foreach (var word in words)
        {
            if (IsWordOnBoard(board, word))
            {
                found.Add(word);
            }
        }

        return found;
    }

    private static bool IsWordOnBoard(char[][] board, string word)
    {
        var used = new bool[board.Length, board[0].Length];

        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (CanMatchWordFromCell(board, word, used, (row, col, 0)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // This repo's own choose/explore/unchoose engine, pruned by a LowercaseTrie
    // built from the whole dictionary: a branch with no matching trie child is
    // pruned before Candidates() ever yields it, so shared prefixes across the
    // dictionary are explored once instead of once per word.
    public static HashSet<string> FindWordsByTrieBacktrack(char[][] board, string[] words)
    {
        var trie = new LowercaseTrie<string>();

        foreach (var word in words)
        {
            trie.Set(word, word);
        }

        var found = new HashSet<string>();

        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                SearchFrom(board, trie.Root, (row, col), found);
            }
        }

        return found;
    }

    private static void SearchFrom(char[][] board, LowercaseTrieNode<string> root, (int Row, int Col) cell, HashSet<string> found)
    {
        var state = new State(board, root, cell.Row, cell.Col);
        Backtrack.Search(state,
            isSolution: s => s.AtWord,
            candidates: s => s.Candidates(),
            choose: (s, p) => s.Choose(p),
            unchoose: (s, p) => s.Unchoose(p),
            onSolution: s => found.Add(s.Word));
    }

    // The walk's own state is which cell it stands on and how much of the word it has
    // matched there; the two advance together on every recursive step, so they are one
    // argument rather than three.
    private static bool CanMatchWordFromCell(char[][] board, string word, bool[,] used, (int Row, int Col, int Index) walk)
    {
        var (row, col, index) = walk;

        if (index == word.Length)
        {
            return true;
        }

        if (IsOutsideBoard(row, col, board) || IsLetterUnavailable((row, col), used, board, word[index]))
        {
            return false;
        }

        used[row, col] = true;

        var matched = CanMatchWordFromCell(board, word, used, (row + 1, col, index + 1))
            || CanMatchWordFromCell(board, word, used, (row - 1, col, index + 1))
            || CanMatchWordFromCell(board, word, used, (row, col + 1, index + 1))
            || CanMatchWordFromCell(board, word, used, (row, col - 1, index + 1));

        used[row, col] = false;
        return matched;
    }

    // Off the board on any of its four edges - there is no cell to match against.
    private static bool IsOutsideBoard(int row, int col, char[][] board)
        => row < 0 || row >= board.Length || col < 0 || col >= board[0].Length;

    // A cell supplies the word's next letter only once, and only if its own letter
    // is that one.
    private static bool IsLetterUnavailable((int Row, int Col) cell, bool[,] used, char[][] board, char expected)
        => used[cell.Row, cell.Col] || board[cell.Row][cell.Col] != expected;

    // Bespoke to LC 212: walks the board and a LowercaseTrieNode in lockstep, so a
    // cell whose letter has no corresponding trie child is never offered as a
    // candidate. LC 79's own State (WordSearchSolution) tracks an index into a
    // single target word instead and cannot be reused here.
    private sealed class State(char[][] board, LowercaseTrieNode<string> root, int startRow, int startCol)
    {
        private readonly bool[,] _used = new bool[board.Length, board[0].Length];
        private readonly Stack<(int Row, int Col, LowercaseTrieNode<string> Node)> _parents = new();
        private LowercaseTrieNode<string> _node = root;
        private bool _started;
        private int Row { get; set; } = startRow;
        private int Col { get; set; } = startCol;

        public bool AtWord => _node.HasValue;
        public string Word => _node.Value;

        public IEnumerable<(int Row, int Col)> Candidates()
        {
            foreach (var p in Neighbors())
            {
                if (IsInBounds(p) && CanExtendWalk(p))
                {
                    yield return p;
                }
            }
        }

        private IEnumerable<(int Row, int Col)> Neighbors()
        {
            if (!_started)
            {
                yield return (Row, Col);
                yield break;
            }

            yield return (Row + 1, Col);
            yield return (Row - 1, Col);
            yield return (Row, Col + 1);
            yield return (Row, Col - 1);
        }

        private bool IsInBounds((int Row, int Col) cell)
            => cell.Row >= 0 && cell.Row < board.Length && cell.Col >= 0 && cell.Col < board[0].Length;

        // A neighbour can extend the walk when the walk has not used it yet and the
        // trie has a child for its letter.
        private bool CanExtendWalk((int Row, int Col) cell) =>
            !_used[cell.Row, cell.Col] && _node.Children[board[cell.Row][cell.Col] - 'a'] is not null;

        public void Choose((int Row, int Col) cell)
        {
            _parents.Push((Row, Col, _node));
            Row = cell.Row;
            Col = cell.Col;
            _used[Row, Col] = true;
            _node = _node.Children[board[Row][Col] - 'a']!;
            _started = true;
        }

        public void Unchoose((int Row, int Col) cell)
        {
            _used[Row, Col] = false;
            (Row, Col, _node) = _parents.Pop();
        }
    }
}
