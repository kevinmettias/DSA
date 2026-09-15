namespace DSAExperimentation.LeetCode.DisplayTableOfFoodOrdersInARestaurant;

// LC 1418's dish name as an order row stores it: the row's food field, and one column of
// the display table. Distinct in type from the table label it is tallied alongside, so a
// cell's (table, food) pair cannot be handed to the counter the wrong way round.
internal readonly record struct FoodName(string Text);
