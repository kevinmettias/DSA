using DSAExperimentation.DataStructures.HashMap;
namespace DSAExperimentation.Tests.LeetCodeCoverage.MajorityElement;
public sealed partial class MajorityElementTests { [Theory] [InlineData(new[]{3,2,3},3)] [InlineData(new[]{2,2,1,1,1,2,2},2)] public void MajorityElement_Examples_ReturnsMajority(int[] nums,int expected)=>Assert.Equal(expected,Majority(nums)); private static int Majority(int[] nums){var counts=new HashMap<int,int>();foreach(var n in nums){counts.TryGetValue(n,out var c);c++;if(c>nums.Length/2)return n;counts.Set(n,c);}return nums[0];} }
