using DSAExperimentation.LeetCode.Solutions.MaximumStudentsTakingExam;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumStudentsTakingExam;

public sealed partial class MaximumStudentsTakingExamReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MaximumStudentsTakingExamReach.IsWithin([new char[36]]));

    [Fact]
    public void IsWithin_SeatsElementsPastTheReach_IsFalse() => Assert.False(MaximumStudentsTakingExamReach.IsWithin([new char[37]]));
}
