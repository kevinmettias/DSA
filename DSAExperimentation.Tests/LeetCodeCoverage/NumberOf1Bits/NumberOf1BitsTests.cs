namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOf1Bits;
public sealed partial class NumberOf1BitsTests { [Theory] [InlineData(11u,3)] [InlineData(128u,1)] public void HammingWeight_Examples_CountsSetBits(uint n,int expected){var c=0;while(n!=0){n&=n-1;c++;}Assert.Equal(expected,c);} }
