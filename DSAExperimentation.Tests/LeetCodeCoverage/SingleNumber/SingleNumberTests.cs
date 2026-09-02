namespace DSAExperimentation.Tests.LeetCodeCoverage.SingleNumber;
public sealed partial class SingleNumberTests { [Theory] [InlineData(new[]{2,2,1},1)] [InlineData(new[]{4,1,2,1,2},4)] public void SingleNumber_Examples_ReturnsUnique(int[] nums,int expected){var actual=nums.Aggregate(0,(a,b)=>a^b);Assert.Equal(expected,actual);} }
