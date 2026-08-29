using DSAExperimentation.DataStructures.Set;
namespace DSAExperimentation.Tests.LeetCodeCoverage.HappyNumber;
public sealed partial class HappyNumberTests { [Theory] [InlineData(19,true)] [InlineData(2,false)] public void IsHappy_Examples_ReturnsExpected(int n,bool expected)=>Assert.Equal(expected,IsHappy(n)); private static bool IsHappy(int n){var seen=new Set<int>();while(n!=1&&seen.TryAdd(n)){var sum=0;while(n>0){var d=n%10;sum+=d*d;n/=10;}n=sum;}return n==1;} }
