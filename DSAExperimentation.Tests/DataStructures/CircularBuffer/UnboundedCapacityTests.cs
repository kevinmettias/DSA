using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.Tests.DataStructures.CircularBuffer;

public sealed partial class UnboundedCapacityTests
{
    [Fact]
    public void IsFullAt_AnyCount_ReturnsFalse() => Assert.False(default(UnboundedCapacity).IsFullAt(int.MaxValue));
}
