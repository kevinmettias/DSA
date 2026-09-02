using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check If It Is a Good Array (LC 1250): Bezout's identity reduces the whole
// problem to "is the array's gcd 1", so the only real performance question left
// is how each pairwise Gcd step itself is computed - repeated subtraction (the
// textbook first algorithm, O(max(a,b)/min(a,b)) per pair) vs. the modulo-based
// Euclidean algorithm (O(log(min(a,b))) per pair), the same "two ways to
// compute the same fold" framing WaterAndJugProblemBenchmarks' GcdFormula uses
// its own private Gcd for. _values are all multiples of 3, guaranteeing the
// running gcd never reaches 1 early, so both benchmarks are forced through
// every element instead of one short-circuiting on the first pair.
[MemoryDiagnoser]
public class CheckIfItIsAGoodArrayBenchmarks
{
    private const int RandomSeed = 6;
    private const int MultipleFactor = 3; // keeps _values multiples of 3 so the running gcd never reaches 1 early
    private const int MaxRandomValueExclusive = 50_000;

    [Params(50, 200)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => MultipleFactor * random.Next(1, MaxRandomValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool SubtractionGcdFold()
    {
        var gcd = _values[0];

        foreach (var value in _values)
        {
            gcd = SubtractionGcd(gcd, value);
        }

        return gcd == 1;
    }

    [Benchmark]
    public bool EuclideanGcdFold()
    {
        var gcd = _values[0];

        foreach (var value in _values)
        {
            gcd = EuclideanGcd(gcd, value);
        }

        return gcd == 1;
    }

    // The textbook first Gcd algorithm: repeatedly subtract the smaller value
    // from the larger until they're equal. Correct, but O(max/min) per pair -
    // a value that's a small multiple of the running gcd forces one subtraction
    // per multiple instead of one division.
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
