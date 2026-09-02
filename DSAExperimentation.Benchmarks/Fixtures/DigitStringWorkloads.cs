namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3461 - a random digit string at LC's own max
// length (10), since Part I's whole point is that both strategies are already
// trivial at that bound.
internal static class DigitStringWorkloads
{
    public static string BuildDigits(int length, int seed)
    {
        var random = new Random(seed);
        var digits = new char[length];

        for (var i = 0; i < length; i++)
        {
            digits[i] = (char)('0' + random.Next(10));
        }

        return new string(digits);
    }
}
