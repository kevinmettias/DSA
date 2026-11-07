using DSAExperimentation.Domain.Mathematics.ExtendedReals;
using Real = DSAExperimentation.Domain.Mathematics.ExtendedReals.AffinelyExtendedReal<long>;

namespace DSAExperimentation.Tests.Domain.Mathematics.ExtendedReals;

// The four forms of arithmetic are exactly the operations the affine extension leaves undefined;
// the three of powers are forms of limits only.
public sealed partial class IndeterminateFormTests
{
    private const int ArithmeticForms = 4;

    public static TheoryData<IndeterminateForm, LimitOperation, Real, Real> Forms => new()
    {
        { IndeterminateForm.ZeroOverZero, LimitOperation.Quotient, new(0), new(0) },
        { IndeterminateForm.InfinityOverInfinity, LimitOperation.Quotient, Real.PositiveInfinity, Real.PositiveInfinity },
        { IndeterminateForm.ZeroTimesInfinity, LimitOperation.Product, new(0), Real.PositiveInfinity },
        { IndeterminateForm.InfinityMinusInfinity, LimitOperation.Difference, Real.PositiveInfinity, Real.PositiveInfinity },
        { IndeterminateForm.ZeroToTheZero, LimitOperation.Power, new(0), new(0) },
        { IndeterminateForm.OneToTheInfinity, LimitOperation.Power, new(1), Real.PositiveInfinity },
        { IndeterminateForm.InfinityToTheZero, LimitOperation.Power, Real.PositiveInfinity, new(0) },
    };

    [Theory]
    [MemberData(nameof(Forms))]
    public void Form_IsItsOperationOnItsTwoLimits(IndeterminateForm form, LimitOperation operation, Real left, Real right) =>
        Assert.Equal(new IndeterminateForm(operation, left, right), form);

    [Fact]
    public void All_IsTheSevenFormsEachOnce() =>
        Assert.Equal(Forms.Select(row => (IndeterminateForm)row[0]), IndeterminateForm.All);

    [Fact]
    public void All_ArithmeticForms_AreUndefinedInTheAffineExtension() =>
        Assert.All(IndeterminateForm.All.Take(ArithmeticForms), form => Assert.False(IsDefined(form)));

    [Fact]
    public void All_ArithmeticFormsComeFirst() =>
        Assert.All(IndeterminateForm.All.Skip(ArithmeticForms), form => Assert.Equal(LimitOperation.Power, form.Operation));

    private static bool IsDefined(IndeterminateForm form) => form.Operation switch
    {
        LimitOperation.Difference => Real.TrySubtract(form.Left, form.Right, out _),
        LimitOperation.Product => Real.TryMultiply(form.Left, form.Right, out _),
        LimitOperation.Quotient => Real.TryDivide(form.Left, form.Right, out _),
        _ => throw new ArgumentOutOfRangeException(nameof(form), form, "A power has no arithmetic here."),
    };
}
