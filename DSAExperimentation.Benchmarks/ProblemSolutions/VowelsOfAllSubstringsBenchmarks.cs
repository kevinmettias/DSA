using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.VowelsOfAllSubstrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are VowelsOfAllSubstringsSolution's, the same methods
// VowelsOfAllSubstringsTests proves correct. [GlobalSetup] builds the random word
// (workload sizing); LeetCode's own input shape is the bare string, so neither
// strategy needs a hoisted overload.
[MemoryDiagnoser]
public class VowelsOfAllSubstringsBenchmarks
{
    // The deterministic word seed this benchmark has always used.
    private const int WordSeed = 3;

    [Params(200, 5_000)]
    public int Length;

    private string _word = null!;

    [GlobalSetup]
    public void Setup() => _word = VowelsOfAllSubstringsWorkloads.BuildRandomLowercaseWord(Length, seed: WordSeed);

    [Benchmark(Baseline = true)]
    public long SubstringScan() => VowelsOfAllSubstringsSolution.CountVowelsBySubstringScan(_word);

    [Benchmark]
    public long ContributionFormula() => VowelsOfAllSubstringsSolution.CountVowelsByContributionFormula(_word);
}
