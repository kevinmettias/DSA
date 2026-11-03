using DSAExperimentation.Domain.Devices.Locks;

namespace DSAExperimentation.Tests.Domain.Devices.Locks;

// The wheel's digits are DecimalDigits', tested beside it; the lock adds only its wheel count.
public sealed partial class FourDigitLockTests
{
    [Fact]
    public void WheelCount_IsFour() => Assert.Equal(4, FourDigitLock.WheelCount);
}
