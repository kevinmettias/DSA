using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordSearchII;

// LeetCode 212. Word Search II: WordSearchTests (LC 79) already proves
// Backtrack.TrySearch's choose/explore/unchoose walk finds one word on a board.
// Finding every word from a whole dictionary at once composes exactly one more
// existing primitive - LowercaseTrie<TValue>, walked node-by-node alongside the
// board so a branch with no matching trie child is pruned before Candidates() ever
// yields it, instead of re-running a fresh single-word search per candidate word.
public sealed partial class WordSearchIITests
{
    [Fact]
    public void FindWords_LeetCodeExample_ReturnsEveryWordPresentOnTheBoard()
    {
        char[][] board =
        [
            ['o', 'a', 'a', 'n'],
            ['e', 't', 'a', 'e'],
            ['i', 'h', 'k', 'r'],
            ['i', 'f', 'l', 'v'],
        ];
        string[] words = ["oath", "pea", "eat", "rain"];

        var found = FindWords(board, words);

        Assert.Equal(["eat", "oath"], found.Order());
    }

    [Fact]
    public void FindWords_NoWordsPresent_ReturnsEmptySet()
    {
        char[][] board = [['a', 'b'], ['c', 'd']];
        string[] words = ["dog", "cat"];

        var found = FindWords(board, words);

        Assert.Empty(found);
    }

    [Fact]
    public void FindWords_OneWordIsAPrefixOfAnother_FindsBothAlongTheSamePath()
    {
        char[][] board = [['a', 'b'], ['c', 'd']];
        string[] words = ["a", "ab"];

        var found = FindWords(board, words);

        Assert.Equal(["a", "ab"], found.Order());
    }

    private static HashSet<string> FindWords(char[][] board, string[] words)
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
