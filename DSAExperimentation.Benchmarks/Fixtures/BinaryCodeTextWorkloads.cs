using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1461. Everything about the strategies themselves
// lives in the LeetCode tier; what stays here is only how the measured text is
// sized and shaped.
internal static class BinaryCodeTextWorkloads
{
    private const char Zero = '0';

    // Every length-k code concatenated once, in order - not random data - so the
    // text deterministically contains ALL 2^k codes. That denies the per-code
    // substring search any early "missing code" exit and forces its full worst-case
    // 2^k searches on every invocation, the same "force the full scan on both
    // sides" intent TwoSum's unreachable target establishes for that benchmark.
    public static string BuildCoveringText(int k)
    {
        var total = 1 << k;
        var builder = new StringBuilder(total * k);

        for (var code = 0; code < total; code++)
        {
            AppendBinary(builder, code, k);
        }

        return builder.ToString();
    }

    private static void AppendBinary(StringBuilder builder, int code, int k)
    {
        for (var bit = k - 1; bit >= 0; bit--)
        {
            builder.Append((char)(Zero + ((code >> bit) & 1)));
        }
    }
}
