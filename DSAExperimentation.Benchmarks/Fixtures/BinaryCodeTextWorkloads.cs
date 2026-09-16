using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1461. Everything about the strategies themselves
// lives in the LeetCode tier; what stays here is only how the measured text is
// sized and shaped.
internal static class BinaryCodeTextWorkloads
{
    private const char Zero = '0';

    // Every code of the given length concatenated once, in order - not random data -
    // so the text deterministically contains ALL 2^codeLength codes. That denies the
    // per-code substring search any early "missing code" exit and forces its full
    // worst-case 2^codeLength searches on every invocation, the same "force the full
    // scan on both sides" intent TwoSum's unreachable target establishes for that
    // benchmark.
    public static string BuildCoveringText(int codeLength)
    {
        var total = 1 << codeLength;
        var builder = new StringBuilder(total * codeLength);

        for (var code = 0; code < total; code++)
        {
            AppendBinary(builder, code, codeLength);
        }

        return builder.ToString();
    }

    private static void AppendBinary(StringBuilder builder, int code, int codeLength)
    {
        for (var bit = codeLength - 1; bit >= 0; bit--)
        {
            builder.Append((char)(Zero + ((code >> bit) & 1)));
        }
    }
}
