using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.NumberTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.NumberTheory;

public sealed partial class PrimesTests
{
    [Fact]
    public void Smallest_IsPrime() => Assert.True(Primality.IsPrime(Primes.Smallest));

    [Fact]
    public void Smallest_HasNoPrimeBelowIt() => Assert.DoesNotContain(Enumerable.Range(0, Primes.Smallest), Primality.IsPrime);
}
