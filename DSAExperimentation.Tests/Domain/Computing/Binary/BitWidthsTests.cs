using DSAExperimentation.Domain.Computing.Binary;

namespace DSAExperimentation.Tests.Domain.Computing.Binary;

public sealed partial class BitWidthsTests
{
    private const int BitsPerByte = 8;

    [Fact]
    public void Int32_IsTheBitsInAnInt() => Assert.Equal(sizeof(int) * BitsPerByte, BitWidths.Int32);
}
