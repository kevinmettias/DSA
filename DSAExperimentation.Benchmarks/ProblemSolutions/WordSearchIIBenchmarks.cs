using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Word Search II (LC 212): the brute-force approach re-runs WordSearchBenchmarks'
// own per-word backtracking DFS (LC 79) once per dictionary word, retracing the
// same board cells from scratch every time - O(words * cells * 4^L). The
// repo-primitive approach instead builds one LowercaseTrie<string> from the whole
// dictionary and walks it alongside a single backtracking pass per starting cell,
// pruning any branch the moment no word shares that prefix, so shared prefixes
// across the dictionary are explored once instead of once per word - the same
// Backtrack.Search choose/explore/unchoose engine WordSearchBenchmarks already
// uses, plus this repo's own LowercaseTrie<TValue> for pruning. Words are random
// (mostly absent from the board) so neither strategy short-circuits, forcing both
// through their real worst-case cost.
[MemoryDiagnoser]
public class WordSearchIIBenchmarks
{
    private const int BoardSize = 8;
    private const int WordLength = 4;
    private const int RandomSeed = 17;
    private const int AlphabetSize = 26;

    [Params(20, 200)]
    public int WordCount;

    private char[][] _board = null!;
    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _board = Enumerable.Range(0, BoardSize)
            .Select(_ => Enumerable.Range(0, BoardSize).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray())
            .ToArray();
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray()))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerWordBruteForce()
    {
        var found = 0;
        foreach (var word in _words)
        {
            if (ExistsOnBoard(word))
            {
                found++;
            }
        }

        return found;
    }

    [Benchmark]
    public int TriePrunedSearch()
    {
        var trie = new LowercaseTrie<string>();
        foreach (var word in _words)
        {
            trie.Set(word, word);
        }

        var found = new HashSet<string>();
        for (var row = 0; row < _board.Length; row++)
        for (var col = 0; col < _board[0].Length; col++)
        {
            SearchFrom(trie.Root, row, col, found);
        }

        return found.Count;
    }

    private bool ExistsOnBoard(string word)
    {
        var used = new bool[_board.Length, _board[0].Length];
        return ScanBoardForWord(new WordSearchContext(word, used));
    }

    private bool ScanBoardForWord(WordSearchContext context)
    {
        for (var row = 0; row < _board.Length; row++)
        for (var col = 0; col < _board[0].Length; col++)
        {
            if (SearchFromCell(context, row, col, 0))
            {
                return true;
            }
        }

        return false;
    }

    private bool SearchFromCell(WordSearchContext context, int r, int c, int i)
    {
        if (i == context.Word.Length)
        {
            return true;
        }

        if (r < 0 || r >= _board.Length || c < 0 || c >= _board[0].Length || context.Used[r, c] || _board[r][c] != context.Word[i])
        {
            return false;
        }

        context.Used[r, c] = true;
        var matched = SearchFromCell(context, r + 1, c, i + 1)
            || SearchFromCell(context, r - 1, c, i + 1)
            || SearchFromCell(context, r, c + 1, i + 1)
            || SearchFromCell(context, r, c - 1, i + 1);
        context.Used[r, c] = false;
        return matched;
    }

    private readonly record struct WordSearchContext(string Word, bool[,] Used);

    private void SearchFrom(LowercaseTrieNode<string> root, int row, int col, HashSet<string> found)
    {
        var state = new State(_board, root, row, col);
        Backtrack.Search(state,
            isSolution: s => s.AtWord,
            candidates: s => s.Candidates(),
            choose: (s, p) => s.Choose(p),
            unchoose: (s, p) => s.Unchoose(p),
            onSolution: s => found.Add(s.Word));
    }

    // See WordSearchIITests.State for the full explanation - repeated here rather
    // than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
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
