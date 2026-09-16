using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountAllValidPickupAndDeliveryOptions;

// LeetCode 1359. Count All Valid Pickup and Delivery Options: count the sequences of
// orderCount pickups and orderCount deliveries in which every P_i precedes its own
// D_i, modulo 1e9+7.
//
// Inserting order i's P_i/D_i pair into an already-valid sequence of length 2(i-1)
// has exactly i * (2i - 1) valid placements - i slots for P_i among the 2i-1 new
// positions, since D_i must land somewhere after wherever P_i lands (LeetCode's own
// editorial derivation) - giving the recurrence f(i) = f(i-1) * i * (2i - 1).
//
// Both strategies compute that recurrence and differ only in direction: bottom-up in
// a single accumulator, or top-down through this repo's own Memoizer, the same
// tabulation-vs-Memoizer contrast UniqueBinarySearchTreesSolution draws on its own
// single-int-state DP.
internal static class CountAllValidPickupAndDeliveryOptionsSolution
{
    // Coefficient in f(i) = f(i-1) * i * (2i - 1): order i's delivery can be inserted
    // into any of the (2i - 1) valid slots left by the existing i-1 orders.
    private const int NewOrderSlotCoefficient = 2;

    // Textbook baseline: walk i from 1 to orderCount carrying one running product,
    // reducing mod 1e9+7 after each multiplication so nothing overflows. BCL
    // arithmetic only.
    public static long CountOrdersByTabulation(int orderCount)
    {
        var ways = 1L;

        for (var i = 1; i <= orderCount; i++)
        {
            ways = ways * i % ModularArithmetic.Modulo * (NewOrderSlotCoefficient * i - 1) % ModularArithmetic.Modulo;
        }

        return ways;
    }

    // Same recurrence driven top-down through Memoizer, so each order count is solved
    // once and shared across every recursive call that needs it.
    public static long CountOrdersByMemoizedRecurrence(int orderCount) =>
        Memoizer.Memoize<int, long>(orderCount, new Ways());

    // The rule, named: the sequences for orderCount orders are the sequences for
    // orderCount - 1 orders times the orderCount * (2 * orderCount - 1) slots the
    // latest order's own pickup/delivery pair can land in, each multiplication
    // reduced mod 1e9+7.
    private sealed class Ways : IRecurrence<int, long>
    {
        public long Replay(int orders, IRecurrence<int, long> rest)
        {
            if (orders == 0)
            {
                return 1L;
            }

            var waysBelow = rest.Replay(orders - 1, rest);
            var placements = NewOrderSlotCoefficient * orders - 1;
            return ReduceModulo(ReduceModulo(waysBelow * orders) * placements);
        }
    }

    // The modular reduction the recurrence applies after each multiplication.
    private static long ReduceModulo(long value) => value % ModularArithmetic.Modulo;
}
