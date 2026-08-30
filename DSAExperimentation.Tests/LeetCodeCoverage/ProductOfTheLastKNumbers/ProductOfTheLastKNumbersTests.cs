using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProductOfTheLastKNumbers;

// LeetCode 1352. Product of the Last K Numbers: a running prefix-product array via
// this repo's own DynamicArray<long> - TimeBasedKeyValueStoreTests' exact
// "parallel/running DynamicArray history, appended in call order" shape. Add(0) resets
// to a fresh single-entry prefix ([1]) instead of ever dividing by zero, so any window
// that would have crossed a zero is caught for free: GetProduct(k) answers 0 whenever
// k exceeds how many numbers have been added since the last reset. Otherwise
// GetProduct(k) is one O(1) division of two prefix entries instead of replaying the
// last k multiplications.
public sealed class ProductOfTheLastKNumbersTests
{
    [Fact]
    public void GetProduct_LeetCodeExampleSequence_ReturnsExpectedProducts()
    {
        var productOfNumbers = new ProductOfNumbers();

        foreach (var num in new[] { 3, 0, 2, 5, 4 })
        {
            productOfNumbers.Add(num);
        }

        Assert.Equal(20, productOfNumbers.GetProduct(2));
        Assert.Equal(40, productOfNumbers.GetProduct(3));
        Assert.Equal(0, productOfNumbers.GetProduct(4));

        productOfNumbers.Add(8);

        Assert.Equal(32, productOfNumbers.GetProduct(2));
    }

    [Fact]
    public void GetProduct_NoZerosEverAdded_ReturnsProductOfEntireWindow()
    {
        var productOfNumbers = new ProductOfNumbers();

        foreach (var num in new[] { 1, 2, 3, 4 })
        {
            productOfNumbers.Add(num);
        }

        Assert.Equal(24, productOfNumbers.GetProduct(4));
        Assert.Equal(12, productOfNumbers.GetProduct(2));
    }

    [Fact]
    public void GetProduct_KEqualsOneRightAfterAZero_ReturnsZero()
    {
        var productOfNumbers = new ProductOfNumbers();

        productOfNumbers.Add(5);
        productOfNumbers.Add(0);

        Assert.Equal(0, productOfNumbers.GetProduct(1));
    }

    private sealed class ProductOfNumbers
    {
        private DynamicArray<long> _prefixProducts = CreateResetPrefix();

        public void Add(int num)
        {
            if (num == 0)
            {
                _prefixProducts = CreateResetPrefix();
                return;
            }

            _prefixProducts.Add(_prefixProducts.Get(_prefixProducts.Count - 1) * num);
        }

        public int GetProduct(int k)
        {
            var sinceReset = _prefixProducts.Count - 1;
            if (k > sinceReset)
            {
                return 0;
            }

            return (int)(_prefixProducts.Get(sinceReset) / _prefixProducts.Get(sinceReset - k));
        }

        private static DynamicArray<long> CreateResetPrefix()
        {
            var prefix = new DynamicArray<long>();
            prefix.Add(1);
            return prefix;
        }
    }
}
