using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Tests.Domain.Locks;

public sealed partial class LockWheelsTests
{
    public static TheoryData<int, string> IndexToZeroPaddedCombination =>
        new() { { 0, "0000" }, { 7, "0007" }, { 42, "0042" }, { 1234, "1234" }, { 9999, "9999" } };

    [Theory]
    [MemberData(nameof(IndexToZeroPaddedCombination))]
    public void Combination_ZeroPadsToOneDigitPerWheel(int index, string expected) =>
        Assert.Equal(expected, LockWheels.Combination(index));

    [Fact]
    public void Combination_IsAlwaysAsLongAsThereAreWheels() =>
        Assert.All(
            new[] { 0, 1, 99, 100, 9_999 },
            i => Assert.Equal(LockWheels.Count, LockWheels.Combination(i).Length));

    [Fact]
    public void CombinationSpace_IsModulusRaisedToTheWheelCount() => Assert.Equal((int)Math.Pow(LockWheels.Modulus, LockWheels.Count), LockWheels.CombinationSpace);

    [Fact]
    public void Combination_IsInjectiveAcrossTheWholeSpace()
    {
        var seen = new HashSet<string>();

        for (var i = 0; i < LockWheels.CombinationSpace; i++)
        {
            Assert.True(seen.Add(LockWheels.Combination(i)));
        }

        Assert.Equal(LockWheels.CombinationSpace, seen.Count);
    }
}
