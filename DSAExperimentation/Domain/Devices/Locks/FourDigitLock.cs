using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Devices.Locks;

/// <summary>The common four-wheel lock with the digits 0 to 9 on each wheel: 10,000 combinations.</summary>
public readonly struct FourDigitLock : ILockSpec<DecimalDigits>
{
    /// <summary>Four.</summary>
    public static int WheelCount => 4;
}
