using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Simplified Fractions (LC 1447): no repo container or algorithm primitive applies
// here (PowXnBenchmarks/CheckIfItIsAGoodArrayTests-style "nothing to compose over
// plain integers" case), so the naive/optimized split is within the one real
// algorithmic choice the problem has - how each pairwise Gcd is computed. Trial
// division counts down from min(numerator, denominator) to find the first common
// divisor (O(min(a,b))) vs. the standard Euclidean algorithm (O(log(min(a,b)))), the
// same private-helper shape CheckIfItIsAGoodArrayTests/NthMagicalNumberTests/
// XOfAKindInADeckOfCardsTests already reuse inline.
[MemoryDiagnoser]
public class SimplifiedFractionsBenchmarks
{
    [Params(200, 2_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int TrialDivisionGcd()
    {
        var count = 0;

        for (var denominator = 2; denominator <= N; denominator++)
        {
            for (var numerator = 1; numerator < denominator; numerator++)
            {
                if (GcdByTrialDivision(numerator, denominator) == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int EuclideanGcd()
    {
        var count = 0;

        for (var denominator = 2; denominator <= N; denominator++)
        {
            for (var numerator = 1; numerator < denominator; numerator++)
            {
                if (GcdByEuclideanAlgorithm(numerator, denominator) == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int GcdByTrialDivision(int a, int b)
    {
        for (var divisor = Math.Min(a, b); divisor >= 1; divisor--)
        {
            if (a % divisor == 0 && b % divisor == 0)
            {
                return divisor;
            }
        }

        return 1;
    }

    private static int GcdByEuclideanAlgorithm(int a, int b) => b == 0 ? a : GcdByEuclideanAlgorithm(b, a % b);
}
