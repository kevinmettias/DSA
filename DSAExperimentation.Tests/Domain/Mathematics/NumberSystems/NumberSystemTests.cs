using DSAExperimentation.Domain.Mathematics.NumberSystems;
using DSAExperimentation.Domain.Mathematics.SetTheory;
using DSAExperimentation.Tests.Domain.Mathematics.NumberSystems.Fixtures;
using Law = DSAExperimentation.Domain.Mathematics.NumberSystems.AlgebraicLaws;

namespace DSAExperimentation.Tests.Domain.Mathematics.NumberSystems;

// The property table is the fact, so it is written out here row by row rather than read back from
// the way the library builds each system from the one before. The Cayley-Dickson rows are then held
// to the arithmetic itself: each doubling's laws are decided from its multiplication (the fixture).
public sealed partial class NumberSystemTests
{
    private const AlgebraicLaws Associative = Law.Associativity | Law.Alternativity;

    // The laws the fixture decides from a doubling's arithmetic.
    private const AlgebraicLaws Decided =
        Law.Order | Law.Commutativity | Law.Associativity | Law.Alternativity | Law.Division;

    // The sedenions' zero divisors: (e1 + e10)(e4 - e15) = 0.
    private const int SedenionDimension = 16;

    private static readonly NumberSystem OrderedCommutative =
        new(Law.Order | Law.Commutativity, Cardinal.AlephNull);

    public static TheoryData<NumberSystem, AlgebraicLaws, Cardinal> PropertyTable => new()
    {
        { NumberSystem.Naturals, Law.Completeness | Law.Order | Law.Commutativity | Associative, Cardinal.AlephNull },
        { NumberSystem.Integers, Law.Subtraction | Law.Completeness | Law.Order | Law.Commutativity | Associative, Cardinal.AlephNull },
        { NumberSystem.Rationals, Law.Subtraction | Law.Division | Law.Order | Law.Commutativity | Associative, Cardinal.AlephNull },
        {
            NumberSystem.Reals,
            Law.Subtraction | Law.Division | Law.Completeness | Law.Order | Law.Commutativity | Associative,
            Cardinal.Continuum
        },
        { NumberSystem.Complex, Law.Subtraction | Law.Division | Law.Completeness | Law.Commutativity | Associative, Cardinal.Continuum },
        { NumberSystem.Quaternions, Law.Subtraction | Law.Division | Law.Completeness | Associative, Cardinal.Continuum },
        { NumberSystem.Octonions, Law.Subtraction | Law.Division | Law.Completeness | Law.Alternativity, Cardinal.Continuum },
        { NumberSystem.Sedenions, Law.Subtraction | Law.Completeness, Cardinal.Continuum },
    };

    // Each doubling, and the laws it gives up.
    public static TheoryData<int, AlgebraicLaws> Doublings => new()
    {
        { 1, Law.Order },
        { 2, Law.Commutativity },
        { 3, Law.Associativity },
        { 4, Law.Alternativity | Law.Division },
    };

    public static TheoryData<int> CayleyDicksonIndices => new() { 0, 1, 2, 3, 4 };

    [Theory]
    [MemberData(nameof(PropertyTable))]
    public void PropertyTable_EachSystem_KeepsExactlyItsLawsAndHasItsSize(
        NumberSystem system, AlgebraicLaws laws, Cardinal cardinality) =>
        Assert.Equal(new NumberSystem(laws, cardinality), system);

    [Fact]
    public void Hierarchy_IsNaturalsIntegersRationalsRealsComplex() =>
        Assert.Equal<NumberSystem>(
            [NumberSystem.Naturals, NumberSystem.Integers, NumberSystem.Rationals, NumberSystem.Reals, NumberSystem.Complex],
            NumberSystem.Hierarchy);

    [Fact]
    public void Hierarchy_EachExtension_AddsSubtractionThenDivisionThenCompletion() =>
        Assert.Equal<AlgebraicLaws>(
            [Law.Subtraction, Law.Division, Law.Completeness, Law.None],
            NumberSystem.Hierarchy.Zip(NumberSystem.Hierarchy.Skip(1), (from, to) => to.Laws & ~from.Laws));

    [Theory]
    [MemberData(nameof(Doublings))]
    public void CayleyDickson_EachDoubling_GivesUpItsLawsAndGainsNone(int doubling, AlgebraicLaws givenUp)
    {
        var (from, to) = (NumberSystem.CayleyDickson[doubling - 1], NumberSystem.CayleyDickson[doubling]);
        Assert.Equal(givenUp, from.Laws & ~to.Laws);
        Assert.Equal(Law.None, to.Laws & ~from.Laws);
    }

    // The k-th doubling has 2^k coordinates, and the laws the table claims for it are the ones its
    // multiplication keeps.
    [Theory]
    [MemberData(nameof(CayleyDicksonIndices))]
    public void CayleyDickson_LawsClaimed_AreTheLawsItsArithmeticKeeps(int doublings) =>
        Assert.Equal(NumberSystem.CayleyDickson[doublings].Laws & Decided, CayleyDicksonLaws.Of(1 << doublings));

    [Fact]
    public void Quaternions_IJ_IsKAndJI_IsMinusK()
    {
        var (i, j) = (CayleyDicksonArithmetic.Basis(4, 1), CayleyDicksonArithmetic.Basis(4, 2));
        Assert.Equal([0L, 0, 0, 1], CayleyDicksonArithmetic.Multiply(i, j));
        Assert.Equal([0L, 0, 0, -1], CayleyDicksonArithmetic.Multiply(j, i));
    }

    [Fact]
    public void Sedenions_HaveZeroDivisors()
    {
        var x = CayleyDicksonArithmetic.Add(Unit(1), Unit(10));
        var y = CayleyDicksonArithmetic.Subtract(Unit(4), Unit(15));
        Assert.True(CayleyDicksonArithmetic.IsZero(CayleyDicksonArithmetic.Multiply(x, y)));
    }

    [Fact]
    public void HasLaws_EveryLawKept_IsTrue() =>
        Assert.True(OrderedCommutative.HasLaws(Law.Order | Law.Commutativity));

    [Fact]
    public void HasLaws_OneLawMissing_IsFalse() =>
        Assert.False(OrderedCommutative.HasLaws(Law.Order | Law.Division));

    [Fact]
    public void HasLaws_NoLaw_IsTrue() => Assert.True(OrderedCommutative.HasLaws(Law.None));

    private static long[] Unit(int index) => CayleyDicksonArithmetic.Basis(SedenionDimension, index);
}
