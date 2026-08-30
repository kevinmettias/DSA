using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ApplyDiscountEveryNOrders;

// LeetCode 1357. Apply Discount Every n Orders: this repo's own HashMap<int,int>
// mapping product id to price - the exact TwoSumTests "one HashMap, O(1) lookup
// per line item" shape - built once from the constructor's products/prices arrays.
// GetBill sums looked-up-price * amount per line item, then applies the discount
// whenever this is the n-th customer since construction (or since the last
// discounted one).
public sealed class ApplyDiscountEveryNOrdersTests
{
    [Fact]
    public void GetBill_LeetCodeExampleSequence_ReturnsExpectedTotalsWithDiscountEveryThirdCall()
    {
        var cashier = new Cashier(3, 50, [1, 2, 3, 4, 5, 6, 7], [100, 200, 300, 400, 300, 200, 100]);

        Assert.Equal(500.0, cashier.GetBill([1, 2], [1, 2]));
        Assert.Equal(4000.0, cashier.GetBill([3, 7], [10, 10]));
        Assert.Equal(800.0, cashier.GetBill([1, 2, 3, 4, 5, 6, 7], [1, 1, 1, 1, 1, 1, 1]));
        Assert.Equal(4000.0, cashier.GetBill([4], [10]));
        Assert.Equal(4000.0, cashier.GetBill([7, 3], [10, 10]));
        Assert.Equal(7350.0, cashier.GetBill([7, 5, 3, 1, 6, 4, 2], [10, 10, 10, 9, 9, 9, 7]));
        Assert.Equal(2500.0, cashier.GetBill([2, 3, 5], [5, 3, 2]));
    }

    [Fact]
    public void GetBill_NEqualsOne_AppliesDiscountToEveryCall()
    {
        var cashier = new Cashier(1, 25, [1, 2], [100, 200]);

        Assert.Equal(75.0, cashier.GetBill([1], [1]));
        Assert.Equal(150.0, cashier.GetBill([2], [1]));
    }

    private sealed class Cashier
    {
        private readonly int _n;
        private readonly int _discount;
        private readonly HashMap<int, int> _priceByProduct = new();
        private int _customerCount;

        public Cashier(int n, int discount, int[] products, int[] prices)
        {
            _n = n;
            _discount = discount;

            for (var i = 0; i < products.Length; i++)
            {
                _priceByProduct.Set(products[i], prices[i]);
            }
        }

        public double GetBill(int[] product, int[] amount)
        {
            double total = 0;
            for (var i = 0; i < product.Length; i++)
            {
                if (_priceByProduct.TryGetValue(product[i], out var price))
                {
                    total += price * amount[i];
                }
            }

            _customerCount++;
            if (_customerCount % _n == 0)
            {
                total -= total * _discount / 100.0;
            }

            return total;
        }
    }
}
