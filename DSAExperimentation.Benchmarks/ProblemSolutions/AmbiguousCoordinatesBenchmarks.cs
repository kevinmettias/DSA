using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AmbiguousCoordinates;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AmbiguousCoordinatesSolution's, the same methods
// AmbiguousCoordinatesTests proves correct. [GlobalSetup] builds the input already
// wrapped in LeetCode's parentheses, so only the enumeration is measured. Both arms
// now return the coordinate list rather than a count - see the solution class's
// note on that deliberate change.
[MemoryDiagnoser]
public class AmbiguousCoordinatesBenchmarks
{
    // LC problem number, used as the deterministic seed for digit-string generation.
    private const int RandomSeed = 816;

    // The leading digit is drawn from 1-9 (never a leading zero).
    private const int NonZeroDigitRange = 9;

    // Every other digit is drawn from 0-9.
    private const int DigitRange = 10;

    [Params(8, 16)]
    public int Length;

    private string _coordinates = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];
        chars[0] = (char)('1' + random.Next(NonZeroDigitRange));

        for (var i = 1; i < Length; i++)
        {
            chars[i] = (char)('0' + random.Next(DigitRange));
        }

        _coordinates = $"({new string(chars)})";
    }

    [Benchmark(Baseline = true)]
    public List<string> RebuildAndRescan() =>
        AmbiguousCoordinatesSolution.FindCoordinatesByRebuildAndRescan(_coordinates);

    [Benchmark]
    public List<string> SliceAndCheckBoundary() =>
        AmbiguousCoordinatesSolution.FindCoordinatesBySliceAndCheck(_coordinates);
}
