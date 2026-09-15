using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfFlowersInFullBloom;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfFlowersInFullBloomSolution's, the same
// methods NumberOfFlowersInFullBloomTests proves correct. [GlobalSetup] builds the
// flower intervals and the arrival times - LeetCode's own input shape, so nothing
// further is prepared for the measured methods; splitting and sorting the endpoint
// arrays is the composed arm's own cost and stays inside it.
[MemoryDiagnoser]
public class NumberOfFlowersInFullBloomBenchmarks
{
    private const int RandomSeed = 2251; // LC problem number
    private const int MaxTimeExclusive = 1_000_000;
    private const int MaxFlowerLengthExclusive = 1_000;

    private int[][] _flowers = [];

    private int[] _persons = [];
    [Params(200, 2_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _flowers = new int[Count][];
        for (var i = 0; i < Count; i++)
        {
            var start = random.Next(0, MaxTimeExclusive);
            var length = random.Next(1, MaxFlowerLengthExclusive);
            _flowers[i] = [start, start + length];
        }

        _persons = Enumerable.Range(0, Count).Select(_ => random.Next(0, MaxTimeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        NumberOfFlowersInFullBloomSolution.FullBloomFlowersByPerPersonScan(_flowers, _persons);

    [Benchmark]
    public int[] SortThenBinarySearch() =>
        NumberOfFlowersInFullBloomSolution.FullBloomFlowersBySortedBounds(_flowers, _persons);
}
