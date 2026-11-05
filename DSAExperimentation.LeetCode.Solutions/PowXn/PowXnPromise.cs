namespace DSAExperimentation.LeetCode.Solutions.PowXn;

// LC 50's promise about its input: "-10^4 <= x^n <= 10^4", and "either x is not zero or n > 0", so the power
// is a modest, defined number whose five printed decimals LeetCode compares within 10^-5. LeetCode's judge
// never poses an input that breaks it. Past it the answer runs to 10^72 or 10^100, where two correct
// strategies part in their fourteenth digit and no fixed tolerance judges them. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class PowXnPromise
{
    private const double LargestMagnitude = 1e4;

    public static bool IsKeptBy(double baseValue, int power)
    {
        var isDefined = baseValue != 0 || power > 0;
        var value = Math.Pow(baseValue, power);

        return isDefined && Math.Abs(value) <= LargestMagnitude;
    }
}
