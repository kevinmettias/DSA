using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reaching Points (LC 780): SubtractiveBackwardReduction undoes one growth step
// at a time (tx -= ty / ty -= tx), the textbook brute force - vs.
// ModuloBackwardReduction, which jumps straight to the remainder in one step
// (tx %= ty / ty %= tx), the same subtraction-vs-modulo contrast that
// separates the naive and fast variants of Euclid's algorithm. No repo
// container or algorithm primitive applies here - there is nothing to compose
// over two running integer pairs, the same "lighter repo-primitive fit" case
// PowXnBenchmarks' exponentiation by squaring already is. Ty is fixed small so
// growing Tx makes SubtractiveBackwardReduction's step count grow with it,
// while ModuloBackwardReduction's stays flat.
[MemoryDiagnoser]
public class ReachingPointsBenchmarks
{
    private const int Sx = 1;
    private const int Sy = 1;
    private const int Ty = 3;

    [Params(10_000, 10_000_000)]
    public int Tx;

    [Benchmark(Baseline = true)]
    public bool SubtractiveBackwardReduction()
    {
        var tx = Tx;
        var ty = Ty;

        while (tx > Sx && ty > Sy)
        {
            if (tx > ty)
            {
                tx -= ty;
            }
            else
            {
                ty -= tx;
            }
        }

        return IsReachable(tx, ty);
    }

    [Benchmark]
    public bool ModuloBackwardReduction()
    {
        var tx = Tx;
        var ty = Ty;

        while (tx > Sx && ty > Sy)
        {
            if (tx > ty)
            {
                tx %= ty;
            }
            else
            {
                ty %= tx;
            }
        }

        return IsReachable(tx, ty);
    }

    private static bool IsReachable(int tx, int ty)
    {
        if (tx == Sx)
        {
            return ty >= Sy && (ty - Sy) % Sx == 0;
        }

        if (ty == Sy)
        {
            return tx >= Sx && (tx - Sx) % Sy == 0;
        }

        return false;
    }
}
