using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution's, the
// same methods
// LexicographicallySmallestStringAfterDeletingDuplicateCharactersTests proves
// correct. The workload is drawn from a small alphabet (5 letters) so almost
// every letter has many duplicate occurrences to consider deleting - a workload
// over the full 26-letter alphabet would let most letters appear once and give
// the repeated-scan baseline nothing to bubble through.
[MemoryDiagnoser]
public class LexicographicallySmallestStringAfterDeletingDuplicateCharactersBenchmarks
{
    private const int RandomSeed = 3816; // LeetCode problem number
    private const int AlphabetSize = 5;

    private string _s = "";

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string RepeatedScan() =>
        LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByRepeatedScan(_s);

    [Benchmark]
    public string MonotonicStack() =>
        LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution.SmallestStringByMonotonicStack(_s);
}
