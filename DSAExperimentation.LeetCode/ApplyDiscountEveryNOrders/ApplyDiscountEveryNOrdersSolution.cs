using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ApplyDiscountEveryNOrders;

// LeetCode 1357. Apply Discount Every n Orders: a Cashier is constructed with a
// product catalogue (parallel products/prices arrays), a discount percentage and a
// cadence n. GetBill(product, amount) sums price * amount over the line items and,
// on every n-th call since construction, knocks the discount percentage off the
// total.
//
// This is a "design" problem - the published interface is a stateful object replayed
// across a call script, not a single pure function - so the strategies here are
// factory methods returning that stateful object, the same CreateBy<Strategy> shape
// TimeBasedKeyValueStoreSolution uses for LC 981.
//
// Both strategies run the identical discount cadence; they differ only in how a line
// item's price is found. CreateByLinearCatalogScan walks the whole catalogue per line
// item - O(catalogue) per lookup - and CreateByHashMapLookup does one O(1) probe into
// this repo's own HashMap<int, int>, the same "one map, O(1) lookup per element"
// contrast TwoSumSolution draws.
internal static class ApplyDiscountEveryNOrdersSolution
{
    // LeetCode states the discount as a percentage of the bill.
    private const double PercentScale = 100.0;

    // Real answer: product id -> price in this repo's HashMap, one probe per line item.
    public static ICashier CreateByHashMapLookup(int n, int discount, int[] products, int[] prices) =>
        new HashMapLookupCashier(n, discount, products, prices);

    // The textbook answer: keep the catalogue as the two arrays it arrived in and scan
    // them for each requested product id. Deliberately written without this repo's
    // primitives - it is the arm the composed strategy above has to justify itself
    // against.
    public static ICashier CreateByLinearCatalogScan(int n, int discount, int[] products, int[] prices) =>
        new LinearCatalogScanCashier(n, discount, products, prices);

    public interface ICashier
    {
        double GetBill(int[] product, int[] amount);
    }

    // The cadence half of the problem, shared by both strategies so the only thing
    // they differ in is the price lookup: count customers, and discount every n-th.
    private struct DiscountCadence(int n, int discount)
    {
        private readonly int _n = n;
        private readonly int _discount = discount;
        private int _customerCount;

        public double Apply(double total)
        {
            _customerCount++;

            return _customerCount % _n == 0 ? total - (total * _discount / PercentScale) : total;
        }
    }

    private sealed class HashMapLookupCashier : ICashier
    {
        private readonly HashMap<int, int> _priceByProduct = new();
        private DiscountCadence _cadence;

        public HashMapLookupCashier(int n, int discount, int[] products, int[] prices)
        {
            _cadence = new DiscountCadence(n, discount);

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

            return _cadence.Apply(total);
        }
    }

    private sealed class LinearCatalogScanCashier(int n, int discount, int[] products, int[] prices) : ICashier
    {
        private readonly int[] _products = products;
        private readonly int[] _prices = prices;
        private DiscountCadence _cadence = new(n, discount);

        public double GetBill(int[] product, int[] amount)
        {
            double total = 0;

            for (var i = 0; i < product.Length; i++)
            {
                total += PriceOf(product[i]) * amount[i];
            }

            return _cadence.Apply(total);
        }

        private int PriceOf(int productId)
        {
            for (var p = 0; p < _products.Length; p++)
            {
                if (_products[p] == productId)
                {
                    return _prices[p];
                }
            }

            return 0;
        }
    }
}
