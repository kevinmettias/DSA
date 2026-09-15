using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.PrintWordsVertically;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrintWordsVerticallySolution's, the same methods
// PrintWordsVerticallyTests proves correct - a List<char> column materialized as a
// padded string and TrimEnd'ed (baseline) against this repo's own
// DynamicArray<char> trimmed in place by popping its tail. Unlike the pre-refactor
// version, which only accumulated each row's length so no output list had to be
// built, both arms now return LeetCode's actual answer and the harness takes its
// row count; the rows were being built either way, so the comparison is still
// about the trimming.
//
// Each arm is handed the prepared word list its hoisted overload takes, so the
// sentence split is charged to [GlobalSetup] rather than to the measured method.
[MemoryDiagnoser]
public class PrintWordsVerticallyBenchmarks
{
    private const int RandomSeed = 1324; // LC problem number
    private const int WordLengthUpperBound = 12; // exclusive upper bound passed to Random.Next
    private const int AlphabetSize = 26;

    private DynamicArray<string> _words = new();

    [Params(50, 500)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = new DynamicArray<string>();

        for (var i = 0; i < WordCount; i++)
        {
            _words.Add(RandomWord(random));
        }
    }

    private static string RandomWord(Random random)
    {
        var wordLength = random.Next(1, WordLengthUpperBound);
        return new([.. Enumerable.Range(0, wordLength).Select(_ => (char)('A' + random.Next(AlphabetSize)))]);
    }

    [Benchmark(Baseline = true)]
    public int ListCharTrimEnd() =>
        PrintWordsVerticallySolution.PrintVerticallyByListTrimEnd(_words).Count;

    [Benchmark]
    public int DynamicArrayTrimTail() =>
        PrintWordsVerticallySolution.PrintVerticallyByDynamicArrayColumns(_words).Count;
}
