namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfTwo;
public sealed partial class PowerOfTwoTests { [Theory] [InlineData(1,true)] [InlineData(16,true)] [InlineData(3,false)] public void IsPowerOfTwo_Examples_ReturnsExpected(int n,bool expected)=>Assert.Equal(expected,n>0&&(n&(n-1))==0); }
