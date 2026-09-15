using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LexicographicallySmallestGeneratedString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LexicographicallySmallestGeneratedStringSolution's,
// the same methods LexicographicallySmallestGeneratedStringTests proves
// correct. str1 is all 'T' and str2 is one repeated character, so every
// consecutive pair of 'T' windows overlaps (gap 1 against a pattern length
// in the hundreds) and is always consistent - the actual case the two
// strategies differ on. This also guarantees neither arm ever returns ""
// early: a random str1/str2 pairing would make an inconsistent, short-
// circuiting overlap likely on the very first few characters, measuring an
// early return rather than the fill cost the two strategies are meant to be
// compared on (DirectFill re-verifies and rewrites all of str2 at every 'T',
// O(n*m); ZFunctionConsistency checks the overlap in O(1) via this repo's
// own ZFunction.Compute and writes only the unwritten suffix, O(n + m)).
[MemoryDiagnoser]
public class LexicographicallySmallestGeneratedStringBenchmarks
{
    private const int PatternLength = 400;

    private string _str1 = "";

    private string _str2 = "";
    [Params(2_000, 8_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _str1 = new string('T', Length);
        _str2 = new string('a', PatternLength);
    }

    [Benchmark(Baseline = true)]
    public string DirectFill() =>
        LexicographicallySmallestGeneratedStringSolution.GenerateStringByDirectFill(
            new LexicographicallySmallestGeneratedStringSolution.ConstraintPattern(_str1),
            new LexicographicallySmallestGeneratedStringSolution.TemplateWord(_str2));

    [Benchmark]
    public string ZFunctionConsistency() =>
        LexicographicallySmallestGeneratedStringSolution.GenerateStringByZFunctionConsistency(
            new LexicographicallySmallestGeneratedStringSolution.ConstraintPattern(_str1),
            new LexicographicallySmallestGeneratedStringSolution.TemplateWord(_str2));
}
