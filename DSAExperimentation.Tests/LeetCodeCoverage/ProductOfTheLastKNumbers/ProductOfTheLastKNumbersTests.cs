using DSAExperimentation.LeetCode.ProductOfTheLastKNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProductOfTheLastKNumbers;

// Harness only: both strategies live in ProductOfTheLastKNumbersSolution and are
// replayed against the same call scripts - LeetCode's published Add/GetProduct
// sequence (including the window that spans the zero, and the query after adding
// more numbers on top of it), a stream with no zero at all, a query for the single
// number added since a zero, a one-number stream, and a stream that opens with a
// zero.
//
// A script step is one published call: IsAdd steps call Add(Value) and assert
// nothing; the rest call GetProduct(Value) and assert Expected, which for those
// steps is the answer LeetCode publishes.
public sealed class ProductOfTheLastKNumbersTests
{
    public static TheoryData<(bool IsAdd, int Value, int Expected)[]> Examples =>
        new()
        {
            {
                [
                    (true, 3, 0),
                    (true, 0, 0),
                    (true, 2, 0),
                    (true, 5, 0),
                    (true, 4, 0),
                    (false, 2, 20),
                    (false, 3, 40),
                    (false, 4, 0),
                    (true, 8, 0),
                    (false, 2, 32),
                ]
            },
            {
                [
                    (true, 1, 0),
                    (true, 2, 0),
                    (true, 3, 0),
                    (true, 4, 0),
                    (false, 4, 24),
                    (false, 2, 12),
                ]
            },
            {
                [
                    (true, 5, 0),
                    (true, 0, 0),
                    (false, 1, 0),
                ]
            },
            {
                [
                    (true, 7, 0),
                    (false, 1, 7),
                ]
            },
            {
                [
                    (true, 0, 0),
                    (true, 3, 0),
                    (true, 4, 0),
                    (false, 2, 12),
                    (false, 1, 4),
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPrefixProductDivision_LeetCodeExamples_ReturnsProductOfLastKNumbers(
        (bool IsAdd, int Value, int Expected)[] calls) =>
        AssertScript(ProductOfTheLastKNumbersSolution.CreateByPrefixProductDivision(), calls);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRawStreamReplay_LeetCodeExamples_ReturnsProductOfLastKNumbers(
        (bool IsAdd, int Value, int Expected)[] calls) =>
        AssertScript(ProductOfTheLastKNumbersSolution.CreateByRawStreamReplay(), calls);

    private static void AssertScript(
        ProductOfTheLastKNumbersSolution.IProductOfNumbers numbers,
        (bool IsAdd, int Value, int Expected)[] calls)
    {
        foreach (var (isAdd, value, expected) in calls)
        {
            if (isAdd)
            {
                numbers.Add(value);
                continue;
            }

            Assert.Equal(expected, numbers.GetProduct(value));
        }
    }
}
