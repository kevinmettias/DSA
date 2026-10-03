using DSAExperimentation.LeetCode.SingleNumberII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SingleNumberIISolution's, the same methods
// SingleNumberIITests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (Baseline() => 1, PrimitiveComposed() => 1) rather
// than a second strategy to reconcile - there was only ever one algorithm here,
// written once in the test and never actually exercised by the benchmark. The
// mod-three sweep scans the array once per bit; the two-bit-counter arm scans it
// once in total.
public class SingleNumberIIBenchmarks
{
    private const int RandomSeed = 137;
    private const int ValueUpperBound = 1_000_000;
    private const int ElementsPerTriple = 3;
    private const int SingletonValue = -1;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int TripleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var triples = Enumerable.Range(0, TripleCount).Select(_ => random.Next(1, ValueUpperBound)).ToArray();

        var values = new List<int>((TripleCount * ElementsPerTriple) + 1);

        foreach (var triple in triples)
        {
            values.Add(triple);
            values.Add(triple);
            values.Add(triple);
        }

        values.Add(SingletonValue);

        _nums = [.. values.OrderBy(_ => random.Next())];
    }

    [Benchmark(Baseline = true)]
    public int BitCountModThree() => SingleNumberIISolution.FindSingleByBitCountModThree(_nums);

    [Benchmark]
    public int TwoBitCounters() => SingleNumberIISolution.FindSingleByTwoBitCounters(_nums);
}
