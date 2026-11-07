using System.Numerics;
using DSAExperimentation.Domain.Computing.FloatingPoint;
using DSAExperimentation.Domain.Mathematics.ExtendedReals;

namespace DSAExperimentation.Tests.Domain.Computing.FloatingPoint;

// IEEE 754's facts about double, read from double's own constants and encoding: NaN is unequal to
// itself, zero has two signs, the infinities absorb finite numbers, double.Epsilon is the smallest
// subnormal and not the machine epsilon, and underflow is gradual.
public sealed partial class Binary64Tests
{
    private const int BitsInADouble = sizeof(double) * 8;

    public static TheoryData<double, FloatingPointClass> Classes => new()
    {
        { BitConverter.Int64BitsToDouble(0x7FF0_0000_0000_0001), FloatingPointClass.SignalingNaN },
        { double.NaN, FloatingPointClass.QuietNaN },
        { double.NegativeInfinity, FloatingPointClass.NegativeInfinity },
        { -1.5, FloatingPointClass.NegativeNormal },
        { -double.Epsilon, FloatingPointClass.NegativeSubnormal },
        { double.NegativeZero, FloatingPointClass.NegativeZero },
        { 0.0, FloatingPointClass.PositiveZero },
        { double.Epsilon, FloatingPointClass.PositiveSubnormal },
        { double.MaxValue, FloatingPointClass.PositiveNormal },
        { double.PositiveInfinity, FloatingPointClass.PositiveInfinity },
    };

    [Theory]
    [MemberData(nameof(Classes))]
    public void ClassOf_EachClassesRepresentative_IsThatClass(double value, FloatingPointClass expected) =>
        Assert.Equal(expected, Binary64.ClassOf(value));

    [Fact]
    public void NaN_IsUnequalToItselfUnderIeeeButEqualUnderDotNetsEquals()
    {
        var (nan, sameNaN) = (double.NaN, double.NaN);
        Assert.False(nan == sameNaN);
        Assert.True(nan != sameNaN);
        Assert.True(nan.Equals(sameNaN));
    }

    [Fact]
    public void Zero_HasTwoSignsThatCompareEqualButDivideApart()
    {
        Assert.True(0.0 == double.NegativeZero);
        AssertSameBits(double.PositiveInfinity, 1 / 0.0);
        AssertSameBits(double.NegativeInfinity, 1 / double.NegativeZero);
        Assert.NotEqual(Bits(0.0), Bits(double.NegativeZero));
    }

    [Fact]
    public void Infinity_AbsorbsFiniteNumbersAndCancelsToNaN()
    {
        AssertSameBits(double.PositiveInfinity, double.PositiveInfinity + double.MaxValue);
        Assert.True(double.IsNaN(double.PositiveInfinity - double.PositiveInfinity));
        Assert.True(double.IsNaN(0.0 * double.PositiveInfinity));
    }

    [Fact]
    public void SmallestSubnormal_IsDotNetsEpsilonTheLowestBitAndHalvesToZero()
    {
        AssertSameBits(double.Epsilon, Binary64.SmallestSubnormal);
        Assert.Equal(1L, Bits(Binary64.SmallestSubnormal));
        AssertSameBits(0.0, Binary64.SmallestSubnormal / 2);
    }

    [Fact]
    public void MachineEpsilon_IsTheGapAboveOneAndHalfOfItRoundsAway()
    {
        AssertSameBits(Binary64.MachineEpsilon, Math.BitIncrement(1.0) - 1.0);
        Assert.NotEqual(Bits(1.0), Bits(1.0 + Binary64.MachineEpsilon));
        AssertSameBits(1.0, 1.0 + (Binary64.MachineEpsilon / 2));
        Assert.NotEqual(Bits(double.Epsilon), Bits(Binary64.MachineEpsilon));
    }

    [Fact]
    public void SmallestNormal_IsNormalAndBelowItUnderflowIsGradual()
    {
        Assert.Equal(FloatingPointClass.PositiveNormal, Binary64.ClassOf(Binary64.SmallestNormal));
        var justBelow = Math.BitDecrement(Binary64.SmallestNormal);
        Assert.Equal(FloatingPointClass.PositiveSubnormal, Binary64.ClassOf(justBelow));
        Assert.Equal(FloatingPointClass.PositiveSubnormal, Binary64.ClassOf(Binary64.SmallestNormal / 2));
        AssertSameBits(Math.ScaleB(1.0, 1 - Binary64.ExponentBias), Binary64.SmallestNormal);
    }

    [Fact]
    public void FieldWidths_FillTheDoubleWithItsSignBit() =>
        Assert.Equal(BitsInADouble, 1 + Binary64.ExponentBits + Binary64.FractionBits);

    [Fact]
    public void FractionBits_AreTheSignificandsLessTheImpliedBit()
    {
        var significandBits = ((IFloatingPoint<double>)1.0).GetSignificandBitLength();
        Assert.Equal(significandBits, Binary64.FractionBits + 1);
    }

    [Fact]
    public void ExponentBias_IsOnesExponentField() =>
        Assert.Equal((long)Binary64.ExponentBias << Binary64.FractionBits, Bits(1.0));

    [Fact]
    public void MaxValue_IsTwoLessEpsilonTimesTwoToTheBias() =>
        AssertSameBits(double.MaxValue, (2 - Binary64.MachineEpsilon) * Math.ScaleB(1.0, Binary64.ExponentBias));

    // IEEE 754 answers the indeterminate forms of arithmetic with NaN, and those of powers with 1.
    [Fact]
    public void IndeterminateForms_ArithmeticIsNaNAndPowersAreOne() =>
        Assert.All(IndeterminateForm.All, form =>
        {
            var value = Evaluated(form);
            Assert.True(form.Operation == LimitOperation.Power ? value == 1.0 : double.IsNaN(value));
        });

    // IEEE 754's facts are about exact bits, so doubles are compared as their encodings.
    private static void AssertSameBits(double expected, double actual) => Assert.Equal(Bits(expected), Bits(actual));

    private static long Bits(double value) => BitConverter.DoubleToInt64Bits(value);

    private static double Evaluated(IndeterminateForm form)
    {
        var (left, right) = (AsDouble(form.Left), AsDouble(form.Right));

        return form.Operation switch
        {
            LimitOperation.Difference => left - right,
            LimitOperation.Product => left * right,
            LimitOperation.Quotient => left / right,
            _ => Math.Pow(left, right),
        };
    }

    private static double AsDouble(AffinelyExtendedReal<long> limit) =>
        limit.TryGetFinite(out var finite) ? finite : double.PositiveInfinity;
}
