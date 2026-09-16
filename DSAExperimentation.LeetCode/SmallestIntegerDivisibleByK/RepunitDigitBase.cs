namespace DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

// The base the repunit's digits are written in, which is also the multiplier in the
// remainder recurrence r -> (r * base + 1) % k that both arms below step by. It is one
// fact of LC 1015, so it is declared once here rather than restated beside each arm.
internal static class RepunitDigitBase
{
    public const int Value = 10;
}
