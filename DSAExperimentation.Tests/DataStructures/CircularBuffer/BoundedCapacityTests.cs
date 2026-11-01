using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.Tests.DataStructures.CircularBuffer;

public sealed partial class BoundedCapacityTests
{
    private const int Capacity = 3;

    [Fact]
    public void Constructor_CapacityBelowOne_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoundedCapacity(0));

    [Fact]
    public void Capacity_ReturnsTheBound() => Assert.Equal(Capacity, new BoundedCapacity(Capacity).Capacity);

    [Fact]
    public void IsFullAt_BelowTheBound_ReturnsFalse() => Assert.False(new BoundedCapacity(Capacity).IsFullAt(Capacity - 1));

    [Fact]
    public void IsFullAt_TheBound_ReturnsTrue() => Assert.True(new BoundedCapacity(Capacity).IsFullAt(Capacity));
}
