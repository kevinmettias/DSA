using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

// The Fermat inverse modulo 10^9 + 7, and modulo 7, where every inverse can be checked by hand.
public sealed partial class ModularInverseTests
{
    private const long Prime = BillionSeven.Value;

    public static TheoryData<long> ValuesToInvert => new() { 2, 3, 12_345, 999_999_999, Prime + 5 };

    public static TheoryData<long, long> ResiduesModSevenToInverses => new() { { 1, 1 }, { 2, 4 }, { 3, 5 }, { 6, 6 } };

    [Theory]
    [MemberData(nameof(ValuesToInvert))]
    public void ByFermat_MultipliedByItsInput_IsOne(long value)
        => Assert.Equal(1, value % Prime * ModularInverse.ByFermat<BillionSeven>(value) % Prime);

    [Theory]
    [MemberData(nameof(ResiduesModSevenToInverses))]
    public void ByFermat_ModuloSeven_IsTheResidueThatMultipliesToOne(long residue, long inverse)
        => Assert.Equal(inverse, ModularInverse.ByFermat<Seven>(residue));

    // 20 / 4 == 5, computed as 20 * inverse(4) modulo the prime.
    [Fact]
    public void ByFermat_DividesExactlyWhereOrdinaryDivisionWould()
        => Assert.Equal(5, 20 * ModularInverse.ByFermat<BillionSeven>(4) % Prime);

    private readonly struct BillionSeven : IPrimeModulus
    {
        public const long Value = 1_000_000_007;

        static long IModulus.Value => Value;
    }

    private readonly struct Seven : IPrimeModulus
    {
        public static long Value => 7;
    }
}
