using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Operations to Make All Array Elements Equal to 1 (LC 2654):
// once no element is already 1, the answer scans every (start, end) window for
// the shortest one whose own running gcd hits 1 - the same O(n^2) gcd-fold shape
// CheckIfItIsAGoodArrayBenchmarks/FindGreatestCommonDivisorOfArrayBenchmarks
// already benchmark, just repeated once per window instead of once for the whole
// array. The only real performance question is still how each pairwise Gcd step
// is computed: repeated subtraction (textbook, O(max/min) per pair) vs. the
// modulo-based Euclidean algorithm (O(log(min)) per pair). _values are all
// multiples of MultipleFactor so no window's running gcd ever reaches 1, forcing
// both arms through the full O(n^2) scan instead of one short-circuiting on an
// early window.
[MemoryDiagnoser]
public class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneBenchmarks
{
    private const int RandomSeed = 2654; // LC problem number
    private const int MultipleFactor = 6; // every value stays a multiple of 6, so no window's gcd ever reaches 1
    private const int MaxRandomValueExclusive = 5_000;

    [Params(30, 100)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => MultipleFactor * random.Next(1, MaxRandomValueExclusive))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SubtractionGcdWindowScan()
    {
        var best = -1;

        for (var start = 0; start < _values.Length; start++)
        {
            var running = _values[start];

            for (var end = start + 1; end < _values.Length; end++)
            {
                running = SubtractionGcd(running, _values[end]);

                if (running == 1)
                {
                    var length = end - start + 1;
                    best = best == -1 ? length : Math.Min(best, length);
                    break;
                }
            }
        }

        return best;
    }

    [Benchmark]
    public int EuclideanGcdWindowScan()
    {
        var best = -1;

        for (var start = 0; start < _values.Length; start++)
        {
            var running = _values[start];

            for (var end = start + 1; end < _values.Length; end++)
            {
                running = EuclideanGcd(running, _values[end]);

                if (running == 1)
                {
                    var length = end - start + 1;
                    best = best == -1 ? length : Math.Min(best, length);
                    break;
                }
            }
        }

        return best;
    }

    // The textbook first Gcd algorithm: repeatedly subtract the smaller value from
    // the larger until they're equal. Correct, but O(max/min) per pair.
    private static int SubtractionGcd(int a, int b)
    {
        while (a != b)
        {
            if (a > b)
            {
                a -= b;
            }
            else
            {
                b -= a;
            }
        }

        return a;
    }

    private static int EuclideanGcd(int a, int b) => b == 0 ? a : EuclideanGcd(b, a % b);
}
