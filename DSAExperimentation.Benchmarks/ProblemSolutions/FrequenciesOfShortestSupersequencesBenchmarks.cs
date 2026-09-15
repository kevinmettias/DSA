using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FrequenciesOfShortestSupersequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FrequenciesOfShortestSupersequencesSolution's, the
// same methods FrequenciesOfShortestSupersequencesTests proves correct. Each arm
// takes the already-built LetterGraph, so parsing LC's word list into letters/edges
// is charged to [GlobalSetup] rather than the subset search being measured.
// WordCount stays under 16*16 = 256, LC's own ceiling for 16 unique letters with
// all-unique 2-character words.
[MemoryDiagnoser]
public class FrequenciesOfShortestSupersequencesBenchmarks
{
    // LC problem number, reused as the deterministic word seed.
    private const int WordSeed = 3435;
    private const int LetterCount = 16;

    private LetterGraph _graph = null!;

    [Params(50, 200)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WordSeed);
        var letters = Enumerable.Range(0, LetterCount).Select(i => (char)('a' + i)).ToArray();
        var words = new HashSet<string>();

        while (words.Count < WordCount)
        {
            var from = letters[random.Next(letters.Length)];
            var to = letters[random.Next(letters.Length)];
            words.Add(new string([from, to]));
        }

        _graph = LetterGraph.Build(words);
    }

    [Benchmark(Baseline = true)]
    public int[][] DfsSkipSet() =>
        FrequenciesOfShortestSupersequencesSolution.SupersequenceFrequenciesByDfsSkipSet(_graph);

    [Benchmark]
    public int[][] TopologicalSort() =>
        FrequenciesOfShortestSupersequencesSolution.SupersequenceFrequenciesByTopologicalSort(_graph);
}
