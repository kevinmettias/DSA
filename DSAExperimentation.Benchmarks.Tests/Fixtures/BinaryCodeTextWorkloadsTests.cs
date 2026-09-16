using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BinaryCodeTextWorkloads (ARCHITECTURE 17.7). The reading depends on the
// text carrying every code of the requested width, each at its own block offset, which is what
// denies the per-code substring search any early "missing code" exit.
public sealed partial class BinaryCodeTextWorkloadsTests
{
    private const int CodeLength = 4;
    private const int CodeCount = 1 << CodeLength;

    [Fact]
    public void BuildCoveringText_CodeLength_ReturnsOneBlockPerCode() =>
        Assert.Equal(
            CodeCount * CodeLength,
            BinaryCodeTextWorkloads.BuildCoveringText(CodeLength).Length);

    [Fact]
    public void BuildCoveringText_EveryBlock_EncodesItsOwnCodeInOrder()
    {
        var text = BinaryCodeTextWorkloads.BuildCoveringText(CodeLength);

        for (var code = 0; code < CodeCount; code++)
        {
            Assert.Equal(Binary(code), text.Substring(code * CodeLength, CodeLength));
        }
    }

    [Fact]
    public void BuildCoveringText_EveryCode_AppearsAsASubstring()
    {
        var text = BinaryCodeTextWorkloads.BuildCoveringText(CodeLength);

        for (var code = 0; code < CodeCount; code++)
        {
            Assert.Contains(Binary(code), text);
        }
    }

    [Fact]
    public void BuildCoveringText_SameCodeLength_ReturnsTheSameText() =>
        Assert.Equal(
            BinaryCodeTextWorkloads.BuildCoveringText(CodeLength),
            BinaryCodeTextWorkloads.BuildCoveringText(CodeLength));

    private static string Binary(int code) => Convert.ToString(code, 2).PadLeft(CodeLength, '0');
}
