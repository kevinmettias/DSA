using DSAExperimentation.LeetCode.Solutions.AsteroidCollision;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AsteroidCollision;

public sealed partial class AsteroidCollisionPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [5, 10, -5], true },
            { [5, 0, -5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoAsteroidHasSizeZero(int[] asteroids, bool expected)
        => Assert.Equal(expected, AsteroidCollisionPromise.IsKeptBy(asteroids));
}
