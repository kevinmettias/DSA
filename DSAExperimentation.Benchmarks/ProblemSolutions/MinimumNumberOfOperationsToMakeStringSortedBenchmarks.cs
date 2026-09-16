using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeStringSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfOperationsToMakeStringSortedSolution's,
// the same methods MinimumNumberOfOperationsToMakeStringSortedTests proves correct.
// They accumulate the identical modular permutation-rank sum and differ only in how
// the "how many remaining letters are smaller" query is answered - a linear O(26)
// scan of the frequency array against this repo's FenwickTree<int,
// SumOperation<int>> - so that query is the only thing being compared. The random
// string is built in [GlobalSetup] because sizing and seeding a workload is a
// measurement decision; it is already LeetCode's own input shape, so neither arm
// needs a hoisted overload.
[MemoryDiagnoser]
public class MinimumNumberOfOperationsToMakeStringSortedBenchmarks
{
    private const int RandomSeed = 1830; // LC problem number
    private const int AlphabetSize = 26;

    private string _text = "";

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(
            Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int LinearFrequencyScan() =>
        MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFrequencyScan(_text);

    [Benchmark]
    public int FenwickTreeSweep() =>
        MinimumNumberOfOperationsToMakeStringSortedSolution.MakeStringSortedByFenwickSweep(_text);
}
