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
            if (ExistsOnBoard(board, word))
            {
                found.Add(word);
            }
        }

        return found;
    }

    private static bool ExistsOnBoard(char[][] board, string word)
    {
        var used = new bool[board.Length, board[0].Length];

        for (var row = 0; row < board.Length; row++)
        for (var col = 0; col < board[0].Length; col++)
        {
            if (SearchFromCell(board, word, used, row, col, 0))
            {
                return true;
            }
        }

        return false;
    }

    private static bool SearchFromCell(char[][] board, string word, bool[,] used, int row, int col, int index)
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

        var matched = SearchFromCell(board, word, used, row + 1, col, index + 1)
            || SearchFromCell(board, word, used, row - 1, col, index + 1)
            || SearchFromCell(board, word, used, row, col + 1, index + 1)
            || SearchFromCell(board, word, used, row, col - 1, index + 1);

        used[row, col] = false;
        return matched;
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
        for (var col = 0; col < board[0].Length; col++)
        {
            SearchFrom(board, trie.Root, row, col, found);
        }

        return found;
    }

    private static void SearchFrom(char[][] board, LowercaseTrieNode<string> root, int row, int col, HashSet<string> found)
    {
        var state = new State(board, root, row, col);
        Backtrack.Search(state,
            isSolution: s => s.AtWord,
            candidates: s => s.Candidates(),
            choose: (s, p) => s.Choose(p),
            unchoose: (s, p) => s.Unchoose(p),
            onSolution: s => found.Add(s.Word));
    }

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
                if (InBounds(p) && !_used[p.Row, p.Col] && _node.Children[board[p.Row][p.Col] - 'a'] is not null)
                {
                    yield return p;
                }
            }
        }

        private bool InBounds((int Row, int Col) p)
            => p.Row >= 0 && p.Row < board.Length && p.Col >= 0 && p.Col < board[0].Length;

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

        public void Choose((int Row, int Col) p)
        {
            _parents.Push((Row, Col, _node));
            Row = p.Row;
            Col = p.Col;
            _used[Row, Col] = true;
            _node = _node.Children[board[Row][Col] - 'a']!;
            _started = true;
        }

        public void Unchoose((int Row, int Col) p)
        {
            _used[Row, Col] = false;
            (Row, Col, _node) = _parents.Pop();
        }
    }
}
