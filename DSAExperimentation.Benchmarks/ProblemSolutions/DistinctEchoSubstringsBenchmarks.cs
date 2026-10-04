using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.DistinctEchoSubstrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistinctEchoSubstringsSolution's, the same methods
// DistinctEchoSubstringsSolutionTests proves correct. _text is random lowercase letters, so
// most candidate pairs fail fast - exactly the shape where RollingHash's O(1) screen
// avoids the baseline's per-pair substring allocation instead of merely relocating
// the same cost.
public class DistinctEchoSubstringsBenchmarks
{
    // The LeetCode problem number, reused as the deterministic random seed.
    private const int RandomSeed = 1316;

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
            chars[i] = LowercaseAlphabet.LetterAt(random.Next(LowercaseAlphabet.Size));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveSubstringComparison() =>
        DistinctEchoSubstringsSolution.CountDistinctEchoesByNaiveSubstringComparison(_text);

    [Benchmark]
    public int RollingHashScreenedEchoCount() =>
        DistinctEchoSubstringsSolution.CountDistinctEchoesByRollingHashScreen(_text);
}
