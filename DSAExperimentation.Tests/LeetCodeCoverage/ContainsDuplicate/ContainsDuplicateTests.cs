using DSAExperimentation.DataStructures.Set;
namespace DSAExperimentation.Tests.LeetCodeCoverage.ContainsDuplicate;
public sealed partial class ContainsDuplicateTests { [Theory] [InlineData(new[]{1,2,3,1},true)] [InlineData(new[]{1,2,3,4},false)] public void ContainsDuplicate_Examples_ReturnsExpected(int[] nums,bool expected)=>Assert.Equal(expected,Contains(nums)); private static bool Contains(int[] nums){var seen=new Set<int>();foreach(var n in nums)if(!seen.TryAdd(n))return true;return false;} }
