using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Mirror Reflection (LC 858): naive O(p) step-by-step unfolding simulation vs.
// O(log(min(p,q))) Euclidean-GCD closed form. No repo container or algorithm
// primitive applies here - there is nothing to compose over two running integers,
// the same "lighter repo-primitive fit" case Reaching Points/Pow(x,n) already
// establish. Q = P - 1 keeps every pair coprime (consecutive integers always are),
// forcing the naive simulation through its full O(p) worst case instead of an
// early exit at a small common factor.
[MemoryDiagnoser]
public class MirrorReflectionBenchmarks
{
    [Params(50_000, 500_000)]
    public int P;

    private int Q => P - 1;

    [Benchmark(Baseline = true)]
    public int SimulatedUnfolding() => SimulateReflection(P, Q);

    [Benchmark]
    public int GcdClosedForm() => MirrorReflection(P, Q);

    // Advances the unfolded-grid crossing count k one square at a time until the
    // ray lands exactly on a horizontal grid line (k*q divisible by p) - the same
    // answer GcdClosedForm reaches directly, just without the Euclidean shortcut.
    private static int SimulateReflection(int p, int q)
    {
        var k = 1;
        while ((long)k * q % p != 0)
        {
            k++;
        }

        var m = (long)k * q / p;
        if (k % 2 == 1 && m % 2 == 0)
        {
            return 0;
        }

        return k % 2 == 1 ? 1 : 2;
    }

    private static int MirrorReflection(int p, int q)
    {
        var g = Gcd(p, q);
        var pPrime = p / g;
        var qPrime = q / g;

        if (pPrime % 2 == 1 && qPrime % 2 == 0)
        {
            return 0;
        }

        return pPrime % 2 == 1 ? 1 : 2;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
