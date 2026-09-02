using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RepeatedDNASequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is RepeatedDNASequencesSolution's fixed-window
// scan, the same method RepeatedDNASequencesTests proves correct. Sequence
// length is swept via [Params]; DnaSequenceWorkloads owns generating a random
// A/C/G/T string of that length so construction is charged to [GlobalSetup]
// rather than to the scan being measured.
[MemoryDiagnoser]
public class RepeatedDNASequencesBenchmarks
{
    private const int Seed = 187;

    [Params(200, 5_000)]
    public int Length;

    private string _sequence = null!;

    [GlobalSetup]
    public void Setup() => _sequence = DnaSequenceWorkloads.BuildSequence(Length, Seed);

    [Benchmark]
    public List<string> FixedWindowSet() => RepeatedDNASequencesSolution.FindByFixedWindowSet(_sequence);
}
