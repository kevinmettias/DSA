using DSAExperimentation.LeetCode.Solutions.ParallelCoursesII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ParallelCoursesII;

public sealed partial class ParallelCoursesIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ParallelCoursesIIReach.IsWithin(10, Enumerable.Repeat(Array.Empty<int>(), 13).ToArray(), 1));

    [Fact]
    public void IsWithin_CourseCountPastTheReach_IsFalse() => Assert.False(ParallelCoursesIIReach.IsWithin(11, Enumerable.Repeat(Array.Empty<int>(), 13).ToArray(), 1));

    [Fact]
    public void IsWithin_RelationsLengthPastTheReach_IsFalse() => Assert.False(ParallelCoursesIIReach.IsWithin(10, Enumerable.Repeat(Array.Empty<int>(), 14).ToArray(), 1));
}
