using DSAExperimentation.LeetCode.SortingTheSentence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortingTheSentenceSolution's, the same methods
// SortingTheSentenceSolutionTests proves correct. Each word is LettersPerWord letters
// and its 1-based position as LC 1859's single trailing digit, so WordCount stops at its
// nine words, and nine such words with their eight spaces make a 197-character sentence,
// inside its 200. Each arm is handed the prepared word array its hoisted overload takes,
// so the shuffle is charged to [GlobalSetup] rather than to the sort being measured.
public class SortingTheSentenceBenchmarks
{
    private const int RandomSeed = 1859; // LC problem number
    private const int LettersPerWord = 20;

    private static readonly string WordBody = new('w', LettersPerWord);

    private string[] _words = [];

    [Params(3, 9)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var positions = Enumerable.Range(1, WordCount).OrderBy(_ => random.Next()).ToArray();
        _words = positions.Select(position => $"{WordBody}{position}").ToArray();
    }

    [Benchmark(Baseline = true)]
    public string PositionScan() => SortingTheSentenceSolution.SortSentenceByPositionScan(_words);

    [Benchmark]
    public string MergeSortByPosition() => SortingTheSentenceSolution.SortSentenceByMergeSort(_words);
}
