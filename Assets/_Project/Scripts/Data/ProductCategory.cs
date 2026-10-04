namespace Stockwell.Data
{
    /// <summary>
    /// The six shelf categories from CLAUDE.md. Values are explicit because they are
    /// written into the save file from Phase 4 on, so reordering must not change them.
    /// </summary>
    public enum ProductCategory
    {
        Drinks = 0,
        Snacks = 1,
        Dairy = 2,
        Produce = 3,
        Canned = 4,
        Household = 5
    }
}
