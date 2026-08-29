using DSAExperimentation.Algorithms.DynamicProgramming;
namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobber;
public sealed partial class HouseRobberTests { [Theory] [InlineData(new[]{1,2,3,1},4)] [InlineData(new[]{2,7,9,3,1},12)] public void Rob_Examples_ReturnsBestNonAdjacentSum(int[] nums,int expected)=>Assert.Equal(expected,Rob(nums)); private static int Rob(int[] nums)=>Memoizer.Memoize<int,int>(0,(i,rob)=>i>=nums.Length?0:Math.Max(rob(i+1),nums[i]+rob(i+2))); }
