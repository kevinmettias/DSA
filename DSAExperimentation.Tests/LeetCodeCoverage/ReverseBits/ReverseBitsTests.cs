namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseBits;
public sealed partial class ReverseBitsTests { [Fact] public void ReverseBits_Example_ReturnsReversedPattern(){Assert.Equal(964176192u,Reverse(43261596u));} private static uint Reverse(uint n){var r=0u;for(var i=0;i<32;i++){r=(r<<1)|(n&1);n>>=1;}return r;} }
