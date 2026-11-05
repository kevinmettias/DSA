using DSAExperimentation.LeetCode.Solutions.MostFrequentPrime;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MostFrequentPrime;

public sealed partial class MostFrequentPrimeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MostFrequentPrimeReach.IsWithin([new int[36]]));

    [Fact]
    public void IsWithin_MatElementsPastTheReach_IsFalse() => Assert.False(MostFrequentPrimeReach.IsWithin([new int[37]]));
}
