using DSAExperimentation.Mechanisms.ExpiringSet;

namespace DSAExperimentation.Tests.Mechanisms.ExpiringSet;

public sealed partial class ExpiringSetTests
{
    private const int TimeToLive = 5;

    [Fact]
    public void Constructor_TimeToLiveBelowOne_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExpiringSet<string>(0));

    [Fact]
    public void CountAlive_BeforeAnyLifeEnds_CountsEveryKey()
    {
        var set = SetHolding(("a", 1), ("b", 2));

        Assert.Equal(2, set.CountAlive(3));
    }

    // A life that started at 1 ends at 1 + 5 = 6: alive at 5, gone at 6 itself.
    [Fact]
    public void CountAlive_AtTheMomentALifeEnds_ItHasEnded()
    {
        var set = SetHolding(("a", 1));

        Assert.Equal(1, set.CountAlive(5));
        Assert.Equal(0, set.CountAlive(6));
    }

    [Fact]
    public void TryRenew_LivingKey_StartsItsLifeAgainFromNow()
    {
        var set = SetHolding(("a", 1));

        Assert.True(set.TryRenew("a", 4));
        Assert.Equal(1, set.CountAlive(8));
        Assert.Equal(0, set.CountAlive(9));
    }

    [Fact]
    public void TryRenew_LapsedKey_ReturnsFalseAndLeavesItOut()
    {
        var set = SetHolding(("a", 1));

        Assert.False(set.TryRenew("a", 6));
        Assert.Equal(0, set.CountAlive(7));
    }

    [Fact]
    public void TryRenew_UnknownKey_ReturnsFalse() => Assert.False(SetHolding(("a", 1)).TryRenew("b", 2));

    // Renewing leaves the key's first life in the queue: counting past that life's end must not
    // let the renewed key go with it.
    [Fact]
    public void CountAlive_PastAReplacedLife_KeepsTheRenewedKey()
    {
        var set = SetHolding(("a", 1), ("b", 2));
        set.TryRenew("a", 3);

        Assert.Equal(1, set.CountAlive(7));
        Assert.True(set.TryRenew("a", 7));
    }

    [Fact]
    public void Set_LapsedKey_StartsANewLife()
    {
        var set = SetHolding(("a", 1));
        set.Set("a", 10);

        Assert.Equal(1, set.CountAlive(14));
    }

    [Fact]
    public void Set_EarlierThanATimeNamedBefore_ThrowsArgumentOutOfRangeException()
    {
        var set = SetHolding(("a", 4));

        Assert.Throws<ArgumentOutOfRangeException>(() => set.Set("b", 3));
    }

    [Fact]
    public void CountAlive_EarlierThanATimeNamedBefore_ThrowsArgumentOutOfRangeException()
    {
        var set = SetHolding(("a", 4));

        Assert.Throws<ArgumentOutOfRangeException>(() => set.CountAlive(3));
    }

    private static ExpiringSet<string> SetHolding(params (string Key, int Start)[] lives)
    {
        var set = new ExpiringSet<string>(TimeToLive);

        foreach (var (key, start) in lives)
        {
            set.Set(key, start);
        }

        return set;
    }
}
