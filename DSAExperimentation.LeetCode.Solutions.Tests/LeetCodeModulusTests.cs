using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.Modular;
using DSAExperimentation.LeetCode.Solutions;

namespace DSAExperimentation.LeetCode.Solutions.Tests;

// LeetCode's modulus is a witness for IPrimeModulus, so the facts that interface states must hold of
// it: it is prime, and two of its residues multiply inside a long.
public sealed partial class LeetCodeModulusTests
{
    // The largest modulus whose (m - 1)^2 fits a long, as IModulus states.
    private const long LargestModulusAProductFits = 3_037_000_499;

    [Fact]
    public void Value_IsTenToTheNinthPlusSeven() => Assert.Equal(1_000_000_007, LeetCodeModulus.Value);

    [Fact]
    public void Value_IsPrime() => Assert.True(Primality.IsPrime(LeetCodeModulus.Value));

    [Fact]
    public void Value_KeepsTheProductOfTwoResiduesInsideALong() => Assert.InRange(LeetCodeModulus.Value, 2, LargestModulusAProductFits);

    // The witness's interface value is the constant every arm reads.
    [Fact]
    public void Value_AsAPrimeModulus_IsTheConstant() => Assert.Equal(LeetCodeModulus.Value, ModulusOf<LeetCodeModulus>());

    private static long ModulusOf<TModulus>()
        where TModulus : struct, IModulus
        => TModulus.Value;
}
