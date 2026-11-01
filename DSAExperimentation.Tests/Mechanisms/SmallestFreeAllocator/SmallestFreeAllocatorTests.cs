using Allocator = DSAExperimentation.Mechanisms.SmallestFreeAllocator.SmallestFreeAllocator;

namespace DSAExperimentation.Tests.Mechanisms.SmallestFreeAllocator;

public sealed partial class SmallestFreeAllocatorTests
{
    private const int First = 1;

    [Fact]
    public void Allocate_NothingReleased_HandsOutFromTheFirstNumberUpward()
    {
        var allocator = new Allocator(First);

        Assert.Equal([1, 2, 3], AllocateTimes(allocator, 3));
    }

    [Fact]
    public void Allocate_AfterReleases_HandsOutTheSmallestReleasedFirst()
    {
        var allocator = AllocatorHolding(4);
        allocator.TryRelease(3);
        allocator.TryRelease(1);

        Assert.Equal([1, 3, 5], AllocateTimes(allocator, 3));
    }

    [Fact]
    public void TryRelease_HeldNumber_ReturnsTrue() => Assert.True(AllocatorHolding(2).TryRelease(2));

    [Fact]
    public void TryRelease_NumberReleasedAlready_ReturnsFalseAndHandsItOutOnce()
    {
        var allocator = AllocatorHolding(2);
        allocator.TryRelease(1);

        Assert.False(allocator.TryRelease(1));
        Assert.Equal([1, 3], AllocateTimes(allocator, 2));
    }

    [Fact]
    public void TryRelease_NumberReleasedThenHandedOutAgain_ReturnsTrue()
    {
        var allocator = AllocatorHolding(2);
        allocator.TryRelease(1);
        _ = allocator.Allocate();

        Assert.True(allocator.TryRelease(1));
    }

    [Theory]
    [MemberData(nameof(NeverIssued))]
    public void TryRelease_NumberNeverHandedOut_ReturnsFalseAndHandsOutNothingNew(int number)
    {
        var allocator = AllocatorHolding(2);

        Assert.False(allocator.TryRelease(number));
        Assert.Equal(3, allocator.Allocate());
    }

    public static TheoryData<int> NeverIssued => new()
    {
        // Below the first number.
        0,
        // The next number it would hand out.
        3,
    };

    private static Allocator AllocatorHolding(int count)
    {
        var allocator = new Allocator(First);
        _ = AllocateTimes(allocator, count);
        return allocator;
    }

    private static int[] AllocateTimes(Allocator allocator, int count)
    {
        var numbers = new int[count];

        for (var i = 0; i < count; i++)
        {
            numbers[i] = allocator.Allocate();
        }

        return numbers;
    }
}
