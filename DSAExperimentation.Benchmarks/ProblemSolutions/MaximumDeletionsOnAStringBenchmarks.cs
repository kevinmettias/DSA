using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumDeletionsOnAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumDeletionsOnAStringSolution's, the same methods
// MaximumDeletionsOnAStringTests proves correct. _text is random lowercase letters,
// so most candidate half-lengths fail fast - exactly the shape where RollingHash's
// O(1) screen avoids the baseline's per-candidate substring allocation instead of
// merely relocating the same cost.
[MemoryDiagnoser]
public class MaximumDeletionsOnAStringBenchmarks
{
    private const int RandomSeed = 2430; // LC problem number
    private const int LowercaseAlphabetSize = 26;

    private string _text = "";

    [Params(80, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(LowercaseAlphabetSize));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveSubstringComparison() =>
        MaximumDeletionsOnAStringSolution.MaxOperationsByNaiveSubstringComparison(_text);

    [Benchmark]
    public int RollingHashScreenedDp() =>
        MaximumDeletionsOnAStringSolution.MaxOperationsByRollingHashScreen(_text);
}
