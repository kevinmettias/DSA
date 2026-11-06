namespace DSAExperimentation.LeetCode.Solutions.OnlineStockSpan;

// LeetCode's own StockSpanner operation, common to every strategy so a harness can replay a call script on
// any of them. Scoped to this problem alone - no other problem asks for next(price)'s span.
internal interface IStockSpanner
{
    int Next(int price);
}
