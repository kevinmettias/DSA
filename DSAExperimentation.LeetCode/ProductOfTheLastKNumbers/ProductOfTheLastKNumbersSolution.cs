using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.ProductOfTheLastKNumbers;

// LeetCode 1352. Product of the Last K Numbers: ProductOfNumbers.Add(num) appends to
// a stream and ProductOfNumbers.GetProduct(k) reports the product of its last k
// entries.
//
// This is a "design" problem - the published interface is a stateful object replayed
// across a call script, not a single pure function - so the strategies here are
// factory methods returning that stateful object, the same CreateBy<Strategy> shape
// TimeBasedKeyValueStoreSolution uses for its two.
//
// CreateByRawStreamReplay keeps the raw numbers and multiplies the last k back
// together on every query: O(k) per query. CreateByPrefixProductDivision keeps a
// running prefix-product DynamicArray<long> - TimeBasedKeyValueStore's exact
// "parallel/running history, appended in call order" shape - and answers with one
// division of two prefix entries: O(1) per query. Add(0) resets that history to a
// fresh single entry ([1]) instead of ever dividing by zero, which handles zeros for
// free: a window that would have crossed a zero is exactly a k larger than the
// entries recorded since the last reset, and those answer 0.
internal static class ProductOfTheLastKNumbersSolution
{
    // Real answer: a running prefix-product array, one division per query.
    public static IProductOfNumbers CreateByPrefixProductDivision() => new PrefixProductNumbers();

    // The textbook answer: keep the raw stream and replay the last k
    // multiplications. Deliberately written over a BCL List<int> without this repo's
    // primitives - it is the arm the composed strategy above has to justify itself
    // against.
    public static IProductOfNumbers CreateByRawStreamReplay() => new RawStreamNumbers();

    public interface IProductOfNumbers
    {
        void Add(int num);

        int GetProduct(int lastCount);
    }

    private sealed class PrefixProductNumbers : IProductOfNumbers
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

        // A prefix array always opens with the empty product, so entry i is the
        // product of the first i numbers since the last reset.
        private static DynamicArray<long> CreateResetPrefix()
        {
            var prefix = new DynamicArray<long>();
            prefix.Add(1);

            return prefix;
        }

        public int GetProduct(int lastCount)
        {
            var sinceReset = _prefixProducts.Count - 1;

            if (lastCount > sinceReset)
            {
                return 0;
            }

            return (int)(_prefixProducts.Get(sinceReset) / _prefixProducts.Get(sinceReset - lastCount));
        }
    }

    private sealed class RawStreamNumbers : IProductOfNumbers
    {
        private readonly List<int> _numbers = [];

        public void Add(int num) => _numbers.Add(num);

        public int GetProduct(int lastCount)
        {
            long product = 1;

            for (var i = _numbers.Count - lastCount; i < _numbers.Count; i++)
            {
                product *= _numbers[i];
            }

            return (int)product;
        }
    }
}
