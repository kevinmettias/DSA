using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DigitStringWorkloads (ARCHITECTURE 17.7): a digit string at the requested
// length - LC 3461's own maximum for Part I - drawn from the decimal digits and rebuilt
// identically from the same seed.
public sealed partial class DigitStringWorkloadsTests
{
    private const int Length = 10;
    private const int Seed = 3461; // LC problem number
    private const char MinDigit = '0';
    private const char MaxDigit = '9';

    [Fact]
    public void BuildDigits_Length_ReturnsOneDigitPerPosition() =>
        Assert.Equal(Length, DigitStringWorkloads.BuildDigits(Length, Seed).Length);

    [Fact]
    public void BuildDigits_EveryCharacter_IsADecimalDigit()
    {
        var digits = DigitStringWorkloads.BuildDigits(Length, Seed);

        Assert.All(digits, digit => Assert.InRange(digit, MinDigit, MaxDigit));
    }

    [Fact]
    public void BuildDigits_SameSeed_ReturnsTheSameDigits() =>
        Assert.Equal(
            DigitStringWorkloads.BuildDigits(Length, Seed),
            DigitStringWorkloads.BuildDigits(Length, Seed));
}
