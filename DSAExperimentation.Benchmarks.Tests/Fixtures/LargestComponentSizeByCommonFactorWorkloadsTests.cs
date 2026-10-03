using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LargestComponentSizeByCommonFactorWorkloads (ARCHITECTURE 17.7). The reading
// depends on every value sharing its factors with a small common pool, so genuine merges occur, and
// on staying inside LC 952's contract: values in [1, 10^5], all of them unique.
public sealed partial class LargestComponentSizeByCommonFactorWorkloadsTests
{
    private const int Length = 400; // the benchmark's largest size
    private const int Seed = 952; // LC problem number
    private const int MinValue = 2;
    private const int MaxValue = 100_000;

    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    [Fact]
    public void Build_Length_ReturnsThatManyDistinctValues()
    {
        var values = Build();

        Assert.Equal(Length, values.Length);
        Assert.Equal(Length, values.Distinct().Count());
    }

    [Fact]
    public void Build_EveryValue_StaysInsideTheProblemsValueRange() =>
        Assert.All(Build(), value => Assert.InRange(value, MinValue, MaxValue));

    [Fact]
    public void Build_EveryValue_FactorsEntirelyOverTheSharedPrimes() =>
        Assert.All(Build(), value => Assert.Equal(1, StripSharedPrimes(value)));

    [Fact]
    public void Build_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(Build(), Build());

    private static int[] Build() => LargestComponentSizeByCommonFactorWorkloads.Build(Length, Seed);

    // What is left of value once every shared prime is divided out of it: one exactly when the
    // pool covers all of its factors.
    private static int StripSharedPrimes(int value)
    {
        var remainder = value;

        foreach (var prime in SharedPrimes)
        {
            while (remainder % prime == 0)
            {
                remainder /= prime;
            }
        }

        return remainder;
    }
}
