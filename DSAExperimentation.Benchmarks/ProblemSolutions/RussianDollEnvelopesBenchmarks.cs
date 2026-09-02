using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RussianDollEnvelopes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RussianDollEnvelopesSolution's, the same methods
// RussianDollEnvelopesTests proves correct. Each arm takes the (Width, Height) pairs
// [GlobalSetup] already prepared, so decoding LeetCode's int[][] shape is not charged to the
// measured method - the hoisted overload RussianDollEnvelopesSolution exposes for exactly that.
[MemoryDiagnoser]
public class RussianDollEnvelopesBenchmarks
{
    private const int RandomSeed = 11;

    [Params(200, 3_000)]
    public int Length;

    private (int Width, int Height)[] _envelopes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _envelopes = Enumerable.Range(0, Length)
            .Select(_ => (Width: random.Next(1, Length), Height: random.Next(1, Length)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveSortAndDp() => RussianDollEnvelopesSolution.MaxEnvelopesByBruteForceDp(_envelopes);

    [Benchmark]
    public int SortThenPatienceSorting() => RussianDollEnvelopesSolution.MaxEnvelopesBySortThenPatience(_envelopes);
}
