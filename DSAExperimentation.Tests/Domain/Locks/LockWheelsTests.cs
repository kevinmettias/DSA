using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Tests.Domain.Locks;

public sealed class LockWheelsTests
{
    [Theory]
    [InlineData(0, "0000")]
    [InlineData(7, "0007")]
    [InlineData(42, "0042")]
    [InlineData(1234, "1234")]
    [InlineData(9999, "9999")]
    public void Combination_ZeroPadsToOneDigitPerWheel(int index, string expected) =>
        Assert.Equal(expected, LockWheels.Combination(index));

    [Fact]
    public void Combination_IsAlwaysAsLongAsThereAreWheels()
    {
        Assert.All(
            new[] { 0, 1, 99, 100, 9_999 },
            i => Assert.Equal(LockWheels.Count, LockWheels.Combination(i).Length));
    }

    [Fact]
    public void CombinationSpace_IsModulusRaisedToTheWheelCount()
    {
        Assert.Equal((int)Math.Pow(LockWheels.Modulus, LockWheels.Count), LockWheels.CombinationSpace);
    }

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
