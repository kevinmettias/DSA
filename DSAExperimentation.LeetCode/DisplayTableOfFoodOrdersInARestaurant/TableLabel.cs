namespace DSAExperimentation.LeetCode.DisplayTableOfFoodOrdersInARestaurant;

// LC 1418's table identifier as an order row stores it: a table number rendered as text,
// which is the row's table field and the display table's row label. Its mirror role is a
// food name, and the two are distinct types because filling one cell compares each
// against ITS OWN field of the order - reversing them matches nothing at all.
internal readonly record struct TableLabel(string Text);
