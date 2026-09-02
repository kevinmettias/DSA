namespace DSAExperimentation.Tests.LeetCodeCoverage.VowelsOfAllSubstrings;

// LeetCode 2063. Vowels of All Substrings: no repo primitive applies - the same
// "closed-form single pass over the bare string" category GasStation/Candy/
// MaximumProductSubarray already established in this repo. Each vowel at index i
// is counted once by every substring that contains it - (i+1) choices of start
// times (n-i) choices of end - so summing that contribution per vowel in one pass
// is exactly the answer, with nothing here to compose over a data structure.
public sealed partial class VowelsOfAllSubstringsTests
{
    [Fact]
    public void CountVowels_Aba_ReturnsSix() => Assert.Equal(6, CountVowels("aba"));

    [Fact]
    public void CountVowels_Abc_ReturnsThree() => Assert.Equal(3, CountVowels("abc"));

    [Fact]
    public void CountVowels_NoVowels_ReturnsZero() => Assert.Equal(0, CountVowels("ltcd"));

    private static long CountVowels(string word)
    {
        var length = word.Length;
        long total = 0;

        for (var i = 0; i < length; i++)
        {
            if (IsVowel(word[i]))
            {
                total += (long)(i + 1) * (length - i);
            }
        }

        return total;
    }

    private static bool IsVowel(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u';
}
