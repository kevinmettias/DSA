using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.WordSearch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordSearchSolution's, the same methods
// WordSearchSolutionTests proves correct. The board and word come from
// WordSearchWorkloads: a seeded two-letter board, and a word read off a seeded walk
// across it but for its last letter, which the board never holds. The answer is
// false, as in LeetCode 79's third example, and neither tracer can learn that
// before following every partial match to the end, so both search their whole
// tree. The word runs to half the board's cells, and both stop at LC 79's caps -
// a 6 x 6 board and a 15-letter word.
public class WordSearchBenchmarks
{
    private const int RandomSeed = 79; // LC problem number
    private const int MaxWordLength = 15;
    private const int CellsPerWordLetter = 2;

    private char[][] _board = [];
    private string _word = "";

    [Params(3, 6)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var wordLength = Math.Min(MaxWordLength, Side * Side / CellsPerWordLetter);
        (_board, _word, _) = WordSearchWorkloads.Build(Side, wordLength, new Random(RandomSeed));
    }

    [Benchmark(Baseline = true)]
    public bool CanTraceWordByBruteForceDfs() => WordSearchSolution.CanTraceWordByBruteForceDfs(_board, _word);

    [Benchmark]
    public bool CanTraceWordByBacktrack() => WordSearchSolution.CanTraceWordByBacktrack(_board, _word);
}
