using DSAExperimentation.Domain.Devices.Locks;

namespace DSAExperimentation.Tests.Domain.Devices.Locks;

public sealed partial class FourDigitLockTests
{
    [Fact]
    public void WheelCount_IsFour() => Assert.Equal(4, FourDigitLock.WheelCount);

    [Fact]
    public void SymbolsPerWheel_AreTheTenDigits() => Assert.Equal(10, FourDigitLock.SymbolsPerWheel);

    [Fact]
    public void FirstSymbol_IsZero() => Assert.Equal('0', FourDigitLock.FirstSymbol);
}
